﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasyCaching.Core;
using Adnc.Infra.Common.Extensions;
using Adnc.Infra.Common.Helper;
using Adnc.Maint.Core.Entities;
using Adnc.Core.Shared.IRepositories;
using Adnc.Application.Shared.Services;
using Adnc.Application.Shared.Dtos;
using Adnc.Maint.Application.Contracts.Dtos;
using Adnc.Maint.Application.Contracts.Services;
using Adnc.Maint.Application.Contracts.Consts;

namespace Adnc.Maint.Application.Services
{
    public class CfgAppService : AbstractAppService, ICfgAppService
    {
        private readonly IEfRepository<SysCfg> _cfgRepository;
        private readonly IEasyCachingProvider _cache;

        public CfgAppService(IEfRepository<SysCfg> cfgRepository
            , IEasyCachingProviderFactory cacheFactory)
        {
            _cfgRepository = cfgRepository;
            _cache = cacheFactory.GetCachingProvider(EasyCachingConsts.RemoteCaching);
        }

        public async Task<AppSrvResult> DeleteAsync(long id)
        {
            await _cfgRepository.DeleteAsync(id);
            return AppSrvResult();
        }

        public async Task<AppSrvResult<PageModelDto<CfgDto>>> GetPagedAsync(CfgSearchPagedDto search)
        {
            Expression<Func<CfgDto, bool>> whereCondition = x => true;
            if (search.Name.IsNotNullOrWhiteSpace())
            {
                whereCondition = whereCondition.And(x => x.Name.Contains(search.Name));
            }
            if (search.Value.IsNotNullOrWhiteSpace())
            {
                whereCondition = whereCondition.And(x => x.Value.Contains(search.Value));
            }

            var allCfgs = await this.GetAllFromCacheAsync();

            var pagedCfgs = allCfgs.Where(whereCondition.Compile())
                                   .OrderByDescending(x => x.CreateTime)
                                   .Skip((search.PageIndex - 1) * search.PageSize)
                                   .Take(search.PageSize)
                                   .ToArray();

            var result = new PageModelDto<CfgDto>()
            {
                Data = pagedCfgs
                ,
                TotalCount = allCfgs.Count
                ,
                PageIndex = search.PageIndex
                ,
                PageSize = search.PageSize
            };

            return result;
        }

        public async Task<AppSrvResult<long>> CreateAsync(CfgCreationDto input)
        {
            var exist = (await this.GetAllFromCacheAsync()).Exists(c => c.Name.EqualsIgnoreCase(input.Name));
            if (exist)
                return Problem(HttpStatusCode.BadRequest, "参数名称已经存在");

            var cfg = Mapper.Map<SysCfg>(input);
            cfg.Id = IdGenerater.GetNextId();

            await _cfgRepository.InsertAsync(cfg);

            return cfg.Id;
        }

        public async Task<AppSrvResult> UpdateAsync(long id, CfgUpdationDto input)
        {
            var exist = (await this.GetAllFromCacheAsync()).Exists(c => c.Name.EqualsIgnoreCase(input.Name) && c.Id != id);
            if (exist)
                return Problem(HttpStatusCode.BadRequest, "参数名称已经存在");

            var entity = Mapper.Map<SysCfg>(input);

            entity.Id = id;

            var updatingProps = UpdatingProps<SysCfg>(x => x.Name, x => x.Value, x => x.Description);

            await _cfgRepository.UpdateAsync(entity, updatingProps);

            return AppSrvResult();
        }

        public async Task<AppSrvResult<CfgDto>> GetAsync(long id)
        {
            return (await this.GetAllFromCacheAsync()).Where(x => x.Id == id).FirstOrDefault();
        }

        private async Task<List<CfgDto>> GetAllFromCacheAsync()
        {
            var cahceValue = await _cache.GetAsync(EasyCachingConsts.CfgListCacheKey, async () =>
            {
                var allCfgs = await _cfgRepository.GetAll(writeDb: true).ToListAsync();
                return Mapper.Map<List<CfgDto>>(allCfgs);
            }, TimeSpan.FromSeconds(EasyCachingConsts.OneYear));

            return cahceValue.Value;
        }
    }
}
