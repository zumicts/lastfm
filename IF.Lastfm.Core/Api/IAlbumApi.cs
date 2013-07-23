﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;

namespace IF.Lastfm.Core.Api
{
    public interface IAlbumApi
    {
        IAuth Auth { get; }

        Task<LastResponse<Album>> GetAlbumInfoAsync(string artist, string album, bool autocorrect = false);
        
        Task<PageResponse<BuyLink>> GetBuyLinksForAlbumAsync(string artist,
            string album,
            CountryCode country,
            bool autocorrect = false);

        Task<PageResponse<Shout>> GetShoutsForAlbumAsync(string artist,
            string album,
            bool autocorrect = false,
            int page = 1,
            int itemsPerPage = LastFm.DefaultPageLength);
        
        Task<PageResponse<Tag>> GetUserTagsForAlbumAsync(string artist, string album, string username, bool autocorrect = false);

        Task<PageResponse<Tag>> GetTopTagsForAlbumAsync(string artist, string album, bool autocorrect = false);

        Task<PageResponse<Album>> SearchForAlbumAsync(string album, int page = 1, int itemsPerPage = LastFm.DefaultPageLength);
    }
}