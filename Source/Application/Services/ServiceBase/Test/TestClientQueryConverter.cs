﻿
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;

using NUnit.Framework;

using Atlanta.Application.Domain.DomainBase.Test;

namespace Atlanta.Application.Services.ServiceBase.Test
{

    [TestFixture]
    public class TestClientQueryConverter : DomainTestBase
    {

        [Test]
        public void TestMapping()
        {
            QueryClassRelation relation = new QueryClassRelation().SetId(3);

            ClientQuery clientQuery =
                ClientQuery.For<QueryClass>()
                    .Add((QueryClass q) => q.Name == "test name")
                    .Add((QueryClass q) => q.Type == QueryClassType.First)
                    .Add((QueryClass q) => q.RelatedTo == relation);

            DetachedCriteria criteria =
                DetachedCriteria.For<QueryClass>()
                    .Add((QueryClass q) => q.Name == "test name")
                    .Add((QueryClass q) => q.Type == QueryClassType.First)
                    .Add((QueryClass q) => q.RelatedTo == relation);

            DetachedCriteria convertedCriteria = ClientQueryConverter.ToDetachedCriteria(clientQuery);

            Assert.AreEqual(criteria.EntityOrClassName, convertedCriteria.EntityOrClassName);
            Assert.AreEqual(criteria.ToString(), convertedCriteria.ToString());
        }

    }

}
