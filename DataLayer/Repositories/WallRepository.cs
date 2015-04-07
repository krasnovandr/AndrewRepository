using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DataLayer.Models;

namespace DataLayer.Repositories
{
    public interface IWallRepository
    {
        List<WallItem> GetWall(string userId);
        WallItem GetWallIem(string userId, int wallItemId);
        void AddWallItem(string userId, WallItem wallItem, IEnumerable<string> songsId);
        void RemoveWallItem(string userId, int wallItemId);
        List<Song> GetWallItemSongs(string userId, int wallItemId);
    }

    public class WallRepository : IWallRepository
    {
        public List<WallItem> GetWall(string userId)
        {
            var wall = new List<WallItem>();
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(m => m.Id == userId);

                if (user != null)
                {
                    wall.AddRange(user.WallItems);
                }
                return wall;
            }
        }

        public WallItem GetWallIem(string userId, int wallItemId)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(m => m.Id == userId);

                if (user != null)
                {
                    return user.WallItems.FirstOrDefault(m => m.WallItemId == wallItemId);
                }
                return null;
            }
        }

        public List<Song> GetWallItemSongs(string userId, int wallItemId)
        {
            var resultSongs = new List<Song>();
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(m => m.Id == userId);

                if (user != null)
                {
                    var wallItem = user.WallItems.FirstOrDefault(m => m.WallItemId == wallItemId);

                    if (wallItem != null)
                    {
                        var songs = db.WallItemsSongs.Where(m => m.WallItemId == wallItemId);

                        foreach (var song in songs)
                        {
                            var findedSong = db.Songs.FirstOrDefault(m => m.SongId == song.SongId);
                            if (findedSong != null)
                            {
                                resultSongs.Add(findedSong);

                            }
                        }

                        return resultSongs;
                    }
                }
                return null;
            }
        }
        public void AddWallItem(string userId, WallItem wallItem, IEnumerable<string> songsId)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(m => m.Id == userId);
                wallItem.AddDate = DateTime.Now;

                if (user != null)
                {
                    user.WallItems.Add(wallItem);
                    db.WallItems.Add(wallItem);
                    db.SaveChanges();
                    if (songsId != null)
                    {
                        foreach (var song in songsId)
                        {
                            db.WallItemsSongs.Add(new WallItemsSongs
                            {
                                SongId = song,
                                WallItemId = wallItem.WallItemId
                            });
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        public void RemoveWallItem(string userId, int wallItemId)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(m => m.Id == userId);

                if (user != null)
                {
                    var wallItem = user.WallItems.FirstOrDefault(m => m.WallItemId == wallItemId);

                    if (wallItem != null)
                    {
                        var itemSongs = db.WallItemsSongs.Where(m => m.WallItemId == wallItemId);
                        db.WallItemsSongs.RemoveRange(itemSongs);
                        user.WallItems.Remove(wallItem);
                        db.WallItems.Remove(wallItem);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
