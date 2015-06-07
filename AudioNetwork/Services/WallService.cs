using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Web;
using AudioNetwork.Helpers;
using AudioNetwork.Models;
using DataLayer.Repositories;

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;
namespace AudioNetwork.Services
{
    public interface IWallService
    {
        List<WallItemViewModel> GetWall(string userId);
        WallItemViewModel GetWallItem(string userId, int wallItemId);
        void AddWallItem(WallItemViewModel wallItemView);
        void RemoveWallItem(string userId, int wallItemId);
        IEnumerable<WallItemViewModel> GetUserNews(string userId);

        IEnumerable<FriendUpdateViewModel> GetFriendUpdates(string userId);
    }

    public class WallService : IWallService
    {
        private readonly IWallRepository _wallRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMusicService _musicService;

        public WallService(
            IWallRepository wallRepository,
            IUserRepository userRepository,
            IMusicService musicService,
            IUserService userService)
        {
            _wallRepository = wallRepository;
            _musicService = musicService;
            _userService = userService;
            _userRepository = userRepository;
        }

        public List<WallItemViewModel> GetWall(string userId)
        {
            //  Capture();
            var wall = _wallRepository.GetWall(userId);
            var wallView = new List<WallItemViewModel>();
            wallView.AddRange(wall.Select(ModelConverters.ToWallItemViewModel));
            foreach (var wallItem in wallView)
            {
                wallItem.AddByUser = ModelConverters.ToUserViewModel(_userRepository.GetUser(wallItem.AddByUserId));
                wallItem.ItemSongs = new List<SongViewModel>();
                wallItem.ItemSongs.AddRange(_wallRepository.GetWallItemSongs(userId, wallItem.WallItemId).Select(ModelConverters.ToSongViewModel));
            }
            return wallView.OrderByDescending(m => m.AddDate).ToList();
        }

        public WallItemViewModel GetWallItem(string userId, int wallItemId)
        {
            var wallItem = ModelConverters.ToWallItemViewModel(_wallRepository.GetWallIem(userId, wallItemId));
            wallItem.AddByUser = ModelConverters.ToUserViewModel(_userRepository.GetUser(wallItem.AddByUserId));

            return wallItem;

        }

        public void AddWallItem(WallItemViewModel wallItemView)
        {
            var songIds = new List<string>();
            if (wallItemView.ItemSongs != null)
            {
                songIds = wallItemView.ItemSongs.Select(m => m.SongId).ToList();
            }
            _wallRepository.AddWallItem(wallItemView.IdUserWall, ModelConverters.ToWallItemModel(wallItemView), songIds);

        }

        public void RemoveWallItem(string userId, int wallItemId)
        {
            _wallRepository.RemoveWallItem(userId, wallItemId);
        }

        public IEnumerable<WallItemViewModel> GetUserNews(string userId)
        {
            var result = new List<WallItemViewModel>();
            var friends = _userRepository.GetFriends(userId);

            foreach (var friend in friends)
            {
                var wall = GetWall(friend.Id);
                foreach (var wallItem in wall)
                {
                    if (wallItem.AddByUserId == friend.Id)
                    {
                        result.Add(wallItem);
                    }
                }

            }

            return result.OrderByDescending(m => m.AddDate);
        }

        public IEnumerable<FriendUpdateViewModel> GetFriendUpdates(string userId)
        {
            var result = new List<FriendUpdateViewModel>();
            var friends = _userService.GetFriends(userId);

            foreach (var friend in friends)
            {
                var friendSongs = _musicService.GetSongsUploadBy(friend.Id);
                var groups = friendSongs.GroupBy(y => (int) (y.AddDate.Ticks/TimeSpan.TicksPerMinute/5)).ToList();

                foreach (var group in groups)
                {
                    var songs = group.ToList();
                    var song = songs.FirstOrDefault();
                    if (song == null)
                    {
                        continue;
                    }

                    result.Add(new FriendUpdateViewModel
                    {
                        Friend = friend,
                        AddDate = song.AddDate,
                        Songs = songs
                    });


                }
            }
            return result.OrderByDescending(m => m.AddDate);

        }

        protected void Capture()
        {
            string url = "http://kinopark.by/film29168.html";
            Thread thread = new Thread(delegate()
            {
                using (WebBrowser browser = new WebBrowser())
                {
                    browser.ScrollBarsEnabled = false;
                    browser.AllowNavigation = true;
                    browser.Navigate(url);
                    browser.Width = 1024;
                    browser.Height = 768;
                    browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted);
                    while (browser.ReadyState != WebBrowserReadyState.Complete)
                    {
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            using (Bitmap bitmap = new Bitmap(browser.Width, browser.Height))
            {
                browser.DrawToBitmap(bitmap, new Rectangle(0, 0, browser.Width, browser.Height));
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(@"E:\Users\Andrei\Documents\AudioNetworkFail\AudioNetwork\Content\Images\thumbnail.png");
                    //var img = Image.FromStream(stream);
                    ////bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    //img.Save("LOL", System.Drawing.Imaging.ImageFormat.Png);
                    ////byte[] bytes = stream.ToArray();
                    // imgScreenShot.Visible = true;
                    // imgScreenShot.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(bytes);
                }
            }
        }
    }
}