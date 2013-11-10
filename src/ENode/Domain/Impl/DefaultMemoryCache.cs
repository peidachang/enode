﻿using System;
using System.Collections.Concurrent;
using ENode.Infrastructure.Serializing;

namespace ENode.Domain.Impl
{
    /// <summary>Default implementation of IMemoryCache which using ConcurrentDictionary.
    /// </summary>
    public class DefaultMemoryCache : IMemoryCache
    {
        private readonly ConcurrentDictionary<string, byte[]> _cacheDict = new ConcurrentDictionary<string, byte[]>();
        private readonly IBinarySerializer _binarySerializer;

        /// <summary>Parameterized constructor.
        /// </summary>
        /// <param name="binarySerializer"></param>
        public DefaultMemoryCache(IBinarySerializer binarySerializer)
        {
            _binarySerializer = binarySerializer;
        }

        /// <summary>Get an aggregate from memory cache.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IAggregateRoot Get(object id, Type type)
        {
            if (id == null) throw new ArgumentNullException("id");
            byte[] value;
            if (_cacheDict.TryGetValue(id.ToString(), out value))
            {
                return _binarySerializer.Deserialize(value, type) as IAggregateRoot;
            }
            return null;
        }
        /// <summary>Get a strong type aggregate from memory cache.
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(object id) where T : class, IAggregateRoot
        {
            if (id == null) throw new ArgumentNullException("id");
            byte[] value;
            return _cacheDict.TryGetValue(id.ToString(), out value) ? _binarySerializer.Deserialize<T>(value) : null;
        }
        /// <summary>Set an aggregate to memory cache.
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set(IAggregateRoot aggregateRoot)
        {
            if (aggregateRoot == null)
            {
                throw new ArgumentNullException("aggregateRoot");
            }
            _cacheDict[aggregateRoot.UniqueId.ToString()] = _binarySerializer.Serialize(aggregateRoot);
        }
    }
}
