using System;
using System.Collections.Generic;
namespace New_Folder.DiscordClasses
{
    public class Embed
    {
        public string title {get;set;}
        public string type {get;set;} = "rich";
        public string description{get;set;}
        public string url{get;set;}
        public int color {get;set;}
        public EmbedFooter footer{get;set;}
        public EmbedImage image{get;set;}
        //public EmbedThumbnail thumbnail{get;set;}
        public EmbedVideo video {get;set;}
        //public EmbedProvider{get;set;}
        //public EmbedAuthor{get;set;}
        public List<EmbedField> fields{get;set;}

    }
}