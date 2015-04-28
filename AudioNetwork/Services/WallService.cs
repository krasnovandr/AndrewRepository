using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AudioNetwork.Helpers;
using AudioNetwork.Models;
using DataLayer.Repositories;

namespace AudioNetwork.Services
{
    public interface IWallService
    {
        List<WallItemViewModel> GetWall(string userId);
        WallItemViewModel GetWallItem(string userId, int wallItemId);
        void AddWallItem(WallItemViewModel wallItemView);
        void RemoveWallItem(string userId, int wallItemId);
    }

    public class WallService:IWallService
    {
        private readonly IWallRepository _wallRepository;
        private readonly IUserRepository _userRepository;

        public WallService(
            IWallRepository wallRepository,
            IUserRepository userRepository)
        {
            _wallRepository = wallRepository;
            _userRepository = userRepository;
        }

        public List<WallItemViewModel> GetWall(string userId)
        {
            var wall = _wallRepository.GetWall(userId);
            var wallView = new List<WallItemViewModel>();
            wallView.AddRange(wall.Select(ModelConverters.ToWallItemViewModel));
            foreach (var wallItem in wallView)
            {
                wallItem.AddByUser = ModelConverters.ToUserViewModel(_userRepository.GetUser(wallItem.AddByUserId));
                wallItem.ItemSongs = new List<SongViewModel>();
                wallItem.ItemSongs.AddRange(_wallRepository.GetWallItemSongs(userId, wallItem.WallItemId).Select(ModelConverters.ToSongViewModel));
            }
          return  wallView.OrderByDescending(m => m.AddDate).ToList();
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
    }
}