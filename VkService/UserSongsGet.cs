using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkService.Models;

namespace VkService
{
    public class UserSongsGet
    {
        private readonly VkApi _vkApi;
        public UserSongsGet(string login, string password)
        {
            var vkAuthorization = new VkAuthorization(login, password);
            _vkApi = vkAuthorization.Authorize();
        }

        public List<SongInfo> GetSongs(bool findLyrics)
        {
            List<SongInfo> songs = new List<SongInfo>();
            if (_vkApi == null)
            {
                return songs;
            }

            var userId = _vkApi.UserId;

            var lyricGetter = new SongLyricsAndInfoGetter(_vkApi);
            if (userId != null)
            {
                var vkSongs = _vkApi.Audio.Get((long)userId);
                //vkSongs = vkSongs.Take(5);
                foreach (var vkSong in vkSongs)
                {
                    var songInfo = new SongInfo
                    {
                        Artist = vkSong.Artist,
                        Genre = vkSong.Genre.ToString(),
                        SongId = Guid.NewGuid().ToString(),
                        SongPath = vkSong.Url.AbsoluteUri,

                        Title = vkSong.Title,
                    };
                    songInfo.Duration = TimeSpan.FromSeconds(vkSong.Duration);
                    if (findLyrics)
                    {
                        songInfo.Lyrics = lyricGetter.GetSongLyrics(vkSong.LyricsId);
                    }

                    songs.Add(songInfo);
                }
            }
            return songs;
        }
    }
}
