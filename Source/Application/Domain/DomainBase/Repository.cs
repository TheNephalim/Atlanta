using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;

using FluentNHibernate.Automapping;

namespace Atlanta.Application.Domain.DomainBase
{

    /// <summary>
    /// Repository implementation that wraps NHibernate session.
    /// </summary>
    public class Repository : IRepository, IDisposable
    {

        private static Configuration _configuration;
        private static ISessionFactory _sessionFactory;

        private ISession        _session;
        private ITransaction    _transaction;

        /// <summary> Constructor </summary>
        public Repository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.OpenSession();
            _transaction = null;
        }

        /// <summary> Singleton session factory </summary>
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    lock (typeof(Repository))
                    {
                        if (_sessionFactory == null)
                        {
                            _configuration = new Configuration();
                            ConfigureMapping(_configuration);
                            _sessionFactory = _configuration.BuildSessionFactory();
                        }
                    }
                }

                return _sessionFactory;
            }
        }

        /// <summary> Underlying NHibernate Session </summary>
        public ISession Session
        {
            get { return _session; }
        }

        /// <summary> Configure domain mapping </summary>
        public static AutoPersistenceModel ConfigureMapping(Configuration configuration)
        {
            AutoPersistenceModel model = Mapping.CreateMapping();
            model.Configure(configuration);
            return model;
        }

        /// <summary> Create a query (ICriteria) </summary>
        public ICriteria CreateQuery<T>()
        {
            return _session.CreateCriteria(typeof(T));
        }

        /// <summary> Create a query (ICriteria) </summary>
        public ICriteria CreateQuery(DetachedCriteria detachedCriteria)
        {
            return detachedCriteria.GetExecutableCriteria(_session);
        }

        /// <summary> Create a query (ICriteria) </summary>
        public ICriteria CreateCountQuery(DetachedCriteria detachedCriteria)
        {
            ICriteria executableCriteria = detachedCriteria.GetExecutableCriteria(_session);
            executableCriteria.ClearOrders();
            return executableCriteria.SetFirstResult(0).SetProjection(Projections.RowCount());
        }

        /// <summary> Create a query (ICriteria) </summary>
        public ICriteria CreateCountQuery(ICriteria criteria)
        {
            ICriteria criteriaClone = CriteriaTransformer.Clone(criteria);
            criteriaClone.ClearOrders();
            return criteriaClone.SetFirstResult(0).SetProjection(Projections.RowCount());
        }

        /// <summary> Load (potentially lazy) an object from the Repository </summary>
        public T Load<T>(long identifier)
        {
            return _session.Load<T>(identifier);
        }

        public void Add<T>(T newObject)
        {
            _session.Save(newObject);
        }

        public void Remove<T>(T existingObject)
        {
            _session.Delete(existingObject);
        }

        /// <summary> Begin a transaction </summary>
        public Repository BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
            return this;
        }

        /// <summary> Commit a transaction </summary>
        public Repository CommitTransaction()
        {
            if (_transaction != null)
            {
                _session.Flush();
                _transaction.Commit();
                _transaction = null;
            }
            return this;
        }

        /// <summary> Rollback a transaction </summary>
        public Repository RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
            return this;
        }

        /// <summary> Flush the session's unit-of-work </summary>
        public void Flush()
        {
            _session.Flush();
        }

        /// <summary> Clear the session's unit-of-work </summary>
        public void Clear()
        {
            _session.Clear();
        }

        /// <summary> Dispose </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction = null;
                }

                if (_session != null)
                {
                    if (_session.IsOpen)
                    {
                        _session.Close();
                    }
                    _session = null;
                }
            }
        }

        /// <summary> Dispose </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }

}
