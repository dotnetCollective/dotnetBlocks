﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared
{
    /// <summary>
    /// operation contains all the information required to execute a business operation in the framework.
    /// </summary>
    /// <remarks>
    /// Operations are similar to a command patter, but are not active. They contain data only and are not active except for self-validation.
    /// </remarks>
    public class Result : Event
    {
        public Result(): base() { }

        public Result(Event relatedTo) : base(relatedTo) { }

        public Result(BusinessActivity relatedTo) : base(relatedTo){ }
    }


    /// <summary>
    /// operation containing <see cref="BusinessEntity"/> required to execute the business operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class Result<TEntity> : Result
        where TEntity : BusinessEntity, new()
    {
        public Result(TEntity? entity, Event relatedTo) : base(relatedTo)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets or sets the entity used in the operation.
        /// </summary>
        /// <value>
        /// The entity to use in the operation.
        /// </value>
        public TEntity? Entity { get; set; }

    }

}
