﻿using System;
using System.Net;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Adnc.Infra.Mapper;

namespace Adnc.Application.Shared.Services
{
    public abstract class AbstractAppService : IAppService
    {
        public IObjectMapper Mapper { get; set; }

        protected AppSrvResult AppSrvResult()
        {
            return new AppSrvResult();
        }

        protected AppSrvResult<TValue> AppSrvResult<TValue>([NotNull] TValue value)
        {
            return new AppSrvResult<TValue>(value);
        }

        protected ProblemDetails Problem(HttpStatusCode? statusCode = null, string detail = null, string title = null, string instance = null, string type = null)
        {
            return new ProblemDetails(statusCode, detail, title, instance, type);
        }

        protected Expression<Func<TEntity, object>>[] UpdatingProps<TEntity>(params Expression<Func<TEntity, object>>[] expressions)
        {
            return expressions;
        }
    }
}
