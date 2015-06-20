﻿using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IF.Lastfm.Core.Enums;

namespace IF.Lastfm.Core.Api.Commands.Artist
{
    [ApiMethodName(LastMethodsNames.artist_getTopAlbums)]
    internal class GetTopAlbumsCommand : GetAsyncCommandBase<PageResponse<LastAlbum>>
    {
        public override string Method
        { get { return LastMethodsNames.artist_getTopAlbums; } }

        public string ArtistName { get; set; }

        public GetTopAlbumsCommand(ILastAuth auth, string artistname)
            : base(auth)
        {
            ArtistName = artistname;
        }

        public override void SetParameters()
        {
            Parameters.Add("artist", ArtistName);

            AddPagingParameters();
            DisableCaching();
        }

        public async override Task<PageResponse<LastAlbum>> HandleResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            LastResponseStatus status;
            if (LastFm.IsResponseValid(json, out status) && response.IsSuccessStatusCode)
            {
                var jtoken = JsonConvert.DeserializeObject<JToken>(json);
                var albumsToken = jtoken.SelectToken("topalbums");
                var itemsToken = albumsToken.SelectToken("album");
                var pageInfoToken = albumsToken.SelectToken("@attr");

                return PageResponse<LastAlbum>.CreateSuccessResponse(itemsToken, pageInfoToken, LastAlbum.ParseJToken, LastPageResultsType.Attr);
            }
            else
            {
                return LastResponse.CreateErrorResponse<PageResponse<LastAlbum>>(status);
            }
        }
    }
}
