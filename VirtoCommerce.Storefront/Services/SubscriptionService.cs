﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi;
using VirtoCommerce.Storefront.Converters.Subscriptions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionModule _subscriptionApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public SubscriptionService(ISubscriptionModule subscriptionApi, IWorkContextAccessor workContextAccessor)
        {
            _subscriptionApi = subscriptionApi;
            _workContextAccessor = workContextAccessor;
        }

        public IPagedList<Subscription> SearchSubscription(SubscriptionSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return Task.Factory.StartNew(() => InnerSearchSubscriptionsAsync(criteria, workContext), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async Task<IPagedList<Subscription>> SearchSubscriptionsAsync(SubscriptionSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return await InnerSearchSubscriptionsAsync(criteria, workContext);
        }

        protected virtual async Task<IPagedList<Subscription>> InnerSearchSubscriptionsAsync(SubscriptionSearchCriteria criteria, WorkContext workContext)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            var result = await _subscriptionApi.SearchSubscriptionsAsync(criteria.ToSearchCriteriaDto());
            return new StaticPagedList<Subscription>(result.Subscriptions.Select(x => x.ToSubscription(workContext.AllCurrencies, workContext.CurrentLanguage)),
                                                     criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
        }
    }
}