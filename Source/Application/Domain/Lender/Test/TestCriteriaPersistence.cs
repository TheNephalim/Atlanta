
using System;
using System.Collections.Generic;

using NUnit.Framework;

using NHibernate.Criterion;
using NHibernate.LambdaExtensions;

using Atlanta.Application.Domain.DomainBase;
using Atlanta.Application.Domain.DomainBase.Test;

namespace Atlanta.Application.Domain.Lender.Test
{

    [TestFixture]
    public class TestCriteriaPersistence : DomainPersistenceTestBase
    {

        private long _libraryId;

        override public void SetUp()
        {
            base.SetUp();

            Library library = Library.InstantiateLibrary();
            Repository.Insert(library);

            library.Create(Media.InstantiateOrphanedMedia(MediaType.Book,  "Book", "A test book"));
            library.Create(Media.InstantiateOrphanedMedia(MediaType.Cd, "CD", "A test cd"));
            library.Create(Media.InstantiateOrphanedMedia(MediaType.Dvd, "DVD", "A test dvd"));

            Repository.Flush();
            Repository.Clear();

            _libraryId = library.Id;
        }

        [Test]
        public void FilterStringAndEnumProperty_Ok()
        {
            Library library = Repository.Load<Library>(_libraryId);

            IList<Media> filteredList;
            {
                ClientQuery filter =
                    ClientQuery.For<Media>()
                        .Add<Media>(m => m.Name == "CD");

                filteredList =
                    library.GetMediaList(filter);

                Assert.AreEqual(1, filteredList.Count);
                Assert.AreEqual("CD", filteredList[0].Name);
            }

            {
                ClientQuery filter =
                    ClientQuery.For<Media>()
                        .Add<Media>(m => m.Type == MediaType.Dvd);

                filteredList =
                    library.GetMediaList(filter);

                Assert.AreEqual(1, filteredList.Count);
                Assert.AreEqual("DVD", filteredList[0].Name);
            }

            {
                ClientQuery filter =
                    ClientQuery.For<Media>()
                        .Add<Media>(m => m.Type == MediaType.Dvd)
                        .Add<Media>(m => m.Name == "CD");

                filteredList =
                    library.GetMediaList(filter);

                Assert.AreEqual(0, filteredList.Count);
            }

            {
                ClientQuery filter =
                    ClientQuery.For<Media>()
                        .Add<Media>(m => m.Type != MediaType.Dvd);

                filteredList =
                    library.GetMediaList(filter);

                Assert.AreEqual(2, filteredList.Count);
                Assert.AreEqual("Book", filteredList[0].Name);
                Assert.AreEqual("CD", filteredList[1].Name);
            }
        }

    }

}

