using System;
using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IF.Lastfm.Core.Enums;

namespace IF.Lastfm.Core.Api.Commands.User
{
    [ApiMethodName(LastMethodsNames.user_getRecentTracks)]
    internal class GetRecentTracksCommand : GetAsyncCommandBase<PageResponse<LastTrack>>
    {
        public override string Method
        { get { return LastMethodsNames.user_getRecentTracks; } }

        public string Username { get; private set; }

        public DateTimeOffset? From { get; set; }

        public GetRecentTracksCommand(ILastAuth auth, string username) : base(auth)
        {
            Username = username;
        }

        public override void SetParameters()
        {
            Parameters.Add("user", Username);
            
            if (From.HasValue)
            {
                Parameters.Add("from", From.Value.AsUnixTime().ToString());
            }

            AddPagingParameters();
            DisableCaching();
        }

        public async override Task<PageResponse<LastTrack>> HandleResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            LastResponseStatus status;
            if (LastFm.IsResponseValid(json, out status) && response.IsSuccessStatusCode)
            {
                var jtoken = JsonConvert.DeserializeObject<JToken>(json).SelectToken("recenttracks");
                var itemsToken = jtoken.SelectToken("track");
                var attrToken = jtoken.SelectToken("@attr");

                return PageResponse<LastTrack>.CreateSuccessResponse(itemsToken, attrToken, LastTrack.ParseJToken, LastPageResultsType.Attr);
            }
            else
            {
                return LastResponse.CreateErrorResponse<PageResponse<LastTrack>>(status);
            }
        }
    }
}