﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Adapters;
using GameCloud.Manager.PluginContract.Requests;
using GameCloud.Manager.PluginContract.Responses;
using GameCloud.UCenter.Common.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TexasPoker.Database;
using TexasPoker.Database.Entities;

namespace TexasPoker.Manager.Api.ApiControllers
{
    /// <summary>
    /// Provide a controller for events.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChipsChangeByManagerEventsController : ApiControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsController" /> class.
        /// </summary>
        /// <param name="database">Indicating the database context.</param>
        /// <param name="settings">Indicating the settings.</param>
        [ImportingConstructor]
        public ChipsChangeByManagerEventsController(TexasPokerDatabaseContext database, Settings settings)
            : base(database, settings)
        {
        }

        /// <summary>
        /// Get event list.
        /// </summary>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <param name="keyword">Indicating the keyword.</param>
        /// <param name="orderby">Indicating the order by name.</param>
        /// <param name="page">Indicating the page number.</param>
        /// <param name="count">Indicating the count.</param>
        /// <returns>Async return event list.</returns>
        [Route("api/events/ChipsChangeByManager")]
        public async Task<PluginPaginationResponse<ChipsChangeByManagerEventEntity>> Post([FromBody]PluginRequestInfo request)
        {
            string keyword = request.GetParameterValue<string>("keyword");
            int page = request.GetParameterValue<int>("page", 1);
            int count = request.GetParameterValue<int>("pageSize", 10);

            Expression<Func<ChipsChangeByManagerEventEntity, bool>> filter = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                filter = e => e.PlayerEtGuid == keyword;
            }

            var total = await this.Database.ChipsChangeByManagerEvents.CountAsync(filter, CancellationToken.None);

            IQueryable<ChipsChangeByManagerEventEntity> queryable = this.Database.ChipsChangeByManagerEvents.Collection.AsQueryable();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            var result = queryable.Skip((page - 1) * count).Take(count).ToList();

            // todo: add orderby support.
            var model = new PluginPaginationResponse<ChipsChangeByManagerEventEntity>
            {
                Page = page,
                PageSize = count,
                Raws = result,
                Total = total
            };

            return model;
        }
    }
}
