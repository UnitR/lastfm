﻿using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Enums;

namespace IF.Lastfm.Core.Api.Commands.Track
{
    [ApiMethodName(LastMethodsNames.track_unlove)]
    internal class UnloveCommand : PostAsyncCommandBase<LastResponse>
    {
        public override string Method
        { get { return LastMethodsNames.track_unlove; } }

        public string TrackName { get; set; }

        public string ArtistName { get; set; }

        public UnloveCommand(ILastAuth auth, string trackname, string artistname)
            : base(auth)
        {
            TrackName = trackname;
            ArtistName = artistname;
        }

        public override void SetParameters()
        {
            Parameters.Add("track", TrackName);
            Parameters.Add("artist", ArtistName);
        }

        public async override Task<LastResponse> HandleResponse(HttpResponseMessage response)
        {
            return await LastResponse.HandleResponse(response);
        }
    }
}