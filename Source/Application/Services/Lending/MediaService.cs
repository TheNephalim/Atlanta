
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using Atlanta.Application.Domain.DomainBase;
using Atlanta.Application.Domain.Lender;

using Atlanta.Application.Services.Interfaces;
using Atlanta.Application.Services.ServiceBase;

namespace Atlanta.Application.Services.Lending
{

    /// <summary>
    ///  Media services
    /// </summary>
    public class MediaService : ServiceObjectBase,
                                IMediaService
    {

        /// <summary>
        ///  Get a list of Media for the system Library
        /// </summary>
        public IList<Media> GetMediaList(   User            user,
                                            DomainCriteria  mediaCriteria)
        {
            IList<Media> mediaList =
                DomainRegistry.Library.GetMediaList(mediaCriteria);

            // temporary solution that
            // creates a copy to serialise
            // across a web-service
            IList<Media> mediaListCopy = new List<Media>();
            foreach (Media sourceMedia in mediaList)
            {
                Media copy = Media.InstantiateOrphanedMedia(sourceMedia.Type, sourceMedia.Name, sourceMedia.Description);
                PropertyInfo propertyInfo = typeof(Media).GetProperty("Id");
                propertyInfo.SetValue(copy, sourceMedia.Id, null);
                mediaListCopy.Add(copy);
            }

            return mediaListCopy;
        }

        /// <summary>
        ///  Create a Media in the system Library
        /// </summary>
        public Media Create(User    user,
                            Media   orphanedMedia)
        {
            return DomainRegistry.Library.Create(orphanedMedia);
        }

        /// <summary>
        ///  Modify an existing Media.
        /// </summary>
        public Media Modify(User    user,
                            Media   modifiedMediaCopy)
        {
            Media loadedMedia = Session.Load<Media>(modifiedMediaCopy.Id);
            return loadedMedia.OwningLibrary.Modify(loadedMedia, modifiedMediaCopy);
        }

        /// <summary>
        ///  Delete the media from the system Library.
        /// </summary>
        public void Delete( User    user,
                            Media   mediaCopy)
        {
            Media loadedMedia = Session.Load<Media>(mediaCopy.Id);
            loadedMedia.OwningLibrary.Delete(loadedMedia);
        }

        /// <summary> RGB - temp method - to be removed</summary>
        public string TempMethodToTestWebServices()
        {
            return "a simple test";
        }

    }


}

