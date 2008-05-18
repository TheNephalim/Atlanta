
using System;
using System.Collections.Generic;

using NHibernate.Criterion;

using NUnit.Framework;

using Atlanta.Application.Domain.DomainBase.Test;

namespace Atlanta.Application.Domain.Lender.Test
{

    [TestFixture]
    public class TestMediaPersistence : DomainPersistenceTestBase
    {

        private long _libraryId;

        override public void SetUp()
        {
            base.SetUp();

            Library library = Library.InstantiateLibrary();
            Session.Save(library);

            library.OwnedMedia.Add(Media.InstantiateMedia(library, MediaType.Book,  "Book", "A test book"));
            library.OwnedMedia.Add(Media.InstantiateMedia(library, MediaType.Cd,    "CD", "A test cd"));
            library.OwnedMedia.Add(Media.InstantiateMedia(library, MediaType.Dvd,   "DVD", "A test dvd"));

            Session.Flush();
            Session.Clear();

            _libraryId = library.Id;
        }

        [Test]
        public void InstantiateOrphanedMedia_Ok()
        {
            Media media = Media.InstantiateOrphanedMedia(MediaType.Dvd, "test", "test description");

            Assert.AreEqual(null, media.OwningLibrary);
            Assert.AreEqual(MediaType.Dvd, media.Type);
            Assert.AreEqual("test", media.Name);
            Assert.AreEqual("test description", media.Description);
        }

        [Test]
        public void ModifyDetails_Ok()
        {
            Media media = Media.InstantiateOrphanedMedia(MediaType.Dvd, "test", "test description");

            media.ModifyDetails(MediaType.Cd, "new name", "new description");

            Assert.AreEqual(null, media.OwningLibrary);
            Assert.AreEqual(MediaType.Cd, media.Type);
            Assert.AreEqual("new name", media.Name);
            Assert.AreEqual("new description", media.Description);
        }

        [Test]
        public void XmlSerialization_Ok()
        {
            Media toSerialize = Media.InstantiateOrphanedMedia(MediaType.Dvd, "test", "test description");
        }

        [Test]
        public void Insert_Load_Ok()
        {
            Library library = Session.Load<Library>(_libraryId);

            Media media = Media.InstantiateMedia(library, MediaType.Dvd, "test", "test description");

            Session.Save(media);
            Assert.IsTrue(media.Id != 0);

            Session.Flush();
            Session.Clear();

            media = Session.Load<Media>(media.Id);

            Assert.AreEqual(MediaType.Dvd, media.Type);
            Assert.AreEqual("test", media.Name);
            Assert.AreEqual("test description", media.Description);
        }

        [Test]
        public void ExampleByFilter_LoadAllBooks()
        {
            Library library = Session.Load<Library>(_libraryId);

            IList<Media> bookMediaInLibrary =
                Session.CreateFilter(library.OwnedMedia, "where type = ?")
                    .SetParameter(0, MediaType.Book)
                    .List<Media>();

            Assert.AreEqual(1, bookMediaInLibrary.Count);
            Assert.AreEqual("Book", bookMediaInLibrary[0].Name);
        }

        [Test]
        public void ExampleByCriteria_LoadAllBooks()
        {
            Library library = Session.Load<Library>(_libraryId);

            IList<Media> bookMediaInLibrary =
                Session.CreateCriteria(typeof(Media))
                    .Add(Expression.Eq("OwningLibrary", library))
                    .Add(Expression.Eq("Type", MediaType.Book))
                    .List<Media>();

            Assert.AreEqual(1, bookMediaInLibrary.Count);
            Assert.AreEqual("Book", bookMediaInLibrary[0].Name);
        }

        [Test]
        public void ExampleByFilter_Paging()
        {
            Library library = Library.InstantiateLibrary();

            for (int i=0; i<20; i++)
            {
                library.OwnedMedia.Add(Media.InstantiateMedia(library, MediaType.Book, "book " + i.ToString(), "book description"));
            }

            Session.Save(library);
            Session.Flush();
            Session.Clear();

            library = Session.Load<Library>(library.Id);

            IList<Media> secondPage5Books =
                Session.CreateFilter(library.OwnedMedia, "")
                    .SetFirstResult(5)
                    .SetMaxResults(5)
                    .List<Media>();

            Assert.AreEqual(5, secondPage5Books.Count);
            Assert.AreEqual("book 5", secondPage5Books[0].Name);
            Assert.AreEqual("book 6", secondPage5Books[1].Name);
            Assert.AreEqual("book 7", secondPage5Books[2].Name);
            Assert.AreEqual("book 8", secondPage5Books[3].Name);
            Assert.AreEqual("book 9", secondPage5Books[4].Name);
        }

    }

}

