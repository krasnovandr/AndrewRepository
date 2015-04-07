using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LastFmServices
{
    public class AlbumTrackInfo
    {
        public AlbumTrackInfo()
        {
            // Tracks = new List<string>();
        }
        public string PicturePath { get; set; }
        public string Content { get; set; }
        //public string ArtistName { get; set; }
        //public string AlbumTiTle { get; set; }
        //public string AlbumPosition { get; set; }
        //public List<string> Tracks { get; set; }
    }
    public class LastFmService
    {
        private const string ApiKey = "b0a06a1bc3e9ccf58d6f5e75dda63452";
        private const string ApiKeyWithparam = "&api_key=" + ApiKey;
        private const string Secret = "025753cf449ff020ef1348f38f6af0a2";
        private const string SessionKey = "876866c283bbcb24666a970042855591 ";
        private const string mainUrl = "http://ws.audioscrobbler.com/2.0/?";
        string methodTrackGetInfo = "method=track.getInfo";
        string methodAlbumGetInfo = "method=album.getinfo";
        string methodTrackSearch = "method=track.search";

        public string GetSessionKey()
        {
            var tokenRequest = (HttpWebRequest)WebRequest.Create("http://ws.audioscrobbler.com/2.0/?method=auth.gettoken&api_key=" + ApiKey);

            var tokenResponse = (HttpWebResponse)tokenRequest.GetResponse();

            string tokenResult = new StreamReader(tokenResponse.GetResponseStream(), Encoding.UTF8).ReadToEnd();

            string token = String.Empty;
            for (int i = tokenResult.IndexOf("<token>") + 7; i < tokenResult.IndexOf("</token"); i++)
            {
                token += tokenResult[i];
            }

            Process s = Process.Start("http://www.last.fm/api/auth/?api_key=" + ApiKey + "&token=" + token);

            string tmp = "api_key" + ApiKey + "methodauth.getSessiontoken" + token + Secret;

            // хешируем это алгоритмом MD5 (думаю, у вас не будет проблем найти его в Интернете)
            string sig = CalculateMD5Hash(tmp);

            string url = "http://ws.audioscrobbler.com/2.0/?method=auth.getSession&token=" + token + "&api_key=" +
                         ApiKey + "&api_sig=" + sig;
            HttpWebRequest sessionRequest =
                (HttpWebRequest)WebRequest.Create(url);
            sessionRequest.AllowAutoRedirect = true;
            HttpWebResponse sessionResponse = null;
            try
            {
                sessionResponse = (HttpWebResponse)sessionRequest.GetResponse();
            }
            catch (Exception exception)
            {


            }

            string sessionResult = new StreamReader(sessionResponse.GetResponseStream(),
                                           Encoding.UTF8).ReadToEnd();
            string sessionKey = String.Empty;
            for (int i = sessionResult.IndexOf("<key>") + 5; i < sessionResult.IndexOf("</key>"); i++)
            {
                sessionKey += sessionResult[i];
            }

            return sessionKey;
        }

        public void getArtistCorrection()
        {
            string method = "method=artist.getcorrection&artist=";
            string url = mainUrl + method + "Pink" + ApiKeyWithparam;
            XDocument xDoc = XDocument.Load((url));


        }

        public AlbumTrackInfo getAlbumInfo(string artist, string album)
        {
            string url = mainUrl + methodAlbumGetInfo + ApiKeyWithparam +
                "&artist=" + artist + "&album=" + album + "&lang=ru";
            XDocument xDoc = new XDocument();

            var albumInfo = new AlbumTrackInfo();
            try
            {
                xDoc = XDocument.Load((url));
            }
            catch (Exception exception)
            {
                return null;
            }


            if (xDoc.Root != null)
            {
                var trackElement = xDoc.Root.Elements("track").ToList();
                var albumElement = trackElement.Elements("album").ToList();

                var image = trackElement.Elements("image").FirstOrDefault(m => (string)m.Attribute("size") == "large");
                
                
                if (image != null)
                {
                    albumInfo.PicturePath = image.Value;
                }
                else
                {
                    image = albumElement.Elements("image").FirstOrDefault(m => (string)m.Attribute("size") == "large");
                    if (image != null)
                    {
                        albumInfo.PicturePath = image.Value;
                    }
                }

                var wiki = trackElement.Elements("wiki").FirstOrDefault();
                if (wiki != null)
                {
                    var content = wiki.Elements("content").FirstOrDefault();

                    if (content != null)
                    {
                        albumInfo.Content = content.Value;
                        albumInfo.Content = albumInfo.Content.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty).Replace("<content>", string.Empty).Replace("</content>", string.Empty);
                    }
                }


            }

            return albumInfo;

        }

        public AlbumTrackInfo getTrackInfo(string artist, string track)
        {
            string url = mainUrl + methodTrackGetInfo + ApiKeyWithparam +
                "&artist=" + artist + "&track=" + track;
            XDocument xDoc = new XDocument();

            var info = new AlbumTrackInfo();
            try
            {
                xDoc = XDocument.Load((url));
            }
            catch (Exception exception)
            {
                return null;
            }


            if (xDoc.Root != null)
            {
                var trackElement = xDoc.Root.Elements("track").ToList();
                var albumElement = trackElement.Elements("album").ToList();

                var image = trackElement.Elements("image").FirstOrDefault(m => (string)m.Attribute("size") == "large");


                if (image != null)
                {
                    info.PicturePath = image.Value;
                }
                else
                {
                    image = albumElement.Elements("image").FirstOrDefault(m => (string)m.Attribute("size") == "large");
                    if (image != null)
                    {
                        info.PicturePath = image.Value;
                    }
                }


                var wiki = trackElement.Elements("wiki").FirstOrDefault();
                if (wiki != null)
                {
                    var content = wiki.Elements("content").FirstOrDefault();

                    if (content != null)
                    {
                        info.Content = content.Value;
                        // albumInfo.Content = albumInfo.Content.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty).Replace("<content>", string.Empty).Replace("</content>", string.Empty);
                    }
                }


            }

            return info;

        }


        public AlbumTrackInfo searchTrackInfo(string track)
        {
            string url = mainUrl + methodTrackSearch +
                "&track=" + track + ApiKeyWithparam + "&limit=1";
            XDocument xDoc = new XDocument();

            var info = new AlbumTrackInfo();
            try
            {
                xDoc = XDocument.Load((url));
            }
            catch (Exception exception)
            {
                return null;
            }


            if (xDoc.Root != null)
            {
                var results = xDoc.Root.Elements("results").ToList();
                var trackTag = results.Elements("trackmatches").Elements("track");//e("results ").ToList();
                if (trackTag == null)
                {
                    return null;
                }
                var image = trackTag.Elements("image").FirstOrDefault(m => (string)m.Attribute("size") == "large");
                if (image != null)
                {
                    info.PicturePath = image.Value;
                }
                else
                {
                    var artistName = trackTag.Elements("artist").FirstOrDefault();

                    if (artistName != null && string.IsNullOrEmpty(artistName.Value) == false)
                    {
                        return  getTrackInfo(artistName.Value, track);
                    }
                }

                var wiki = trackTag.Elements("wiki").FirstOrDefault();
                if (wiki != null)
                {
                    var content = wiki.Elements("content").FirstOrDefault();

                    if (content != null)
                    {
                        info.Content = content.Value;
                        // albumInfo.Content = albumInfo.Content.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty).Replace("<content>", string.Empty).Replace("</content>", string.Empty);
                    }
                }


            }

            return info;

        }


        public string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

