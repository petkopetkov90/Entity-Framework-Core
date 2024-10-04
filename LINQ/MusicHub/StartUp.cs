using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MusicHub
{
    using Data;
    using Initializer;
    using System;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Producers
                .Include(p => p.Albums)
                .ThenInclude(a => a.Songs)
                .ThenInclude(s => s.Writer)
                .FirstOrDefault(p => p.Id == producerId)!
                .Albums.Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        SongPrice = s.Price,
                        SongWriter = s.Writer.Name
                    }).AsEnumerable()
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.SongWriter),
                    TotalPrice = a.Price
                }).AsEnumerable()
                .OrderByDescending(a => a.TotalPrice);


            StringBuilder sb = new StringBuilder();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int counter = 0;

                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{++counter}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:f2}");
                    sb.AppendLine($"---Writer: {song.SongWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var span = new TimeSpan(0, 0, duration);
            var songs = context.Songs
                .Where(s => s.Duration > span)
                .Select(s => new
                {
                    SongName = s.Name,
                    SongPerformers = s.SongPerformers
                        .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                        .OrderBy(sp => sp)
                        .ToList(),
                    SongWriter = s.Writer.Name,
                    Producer = s.Album.Producer.Name,
                    Durations = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.SongWriter)
                .ToList();


            StringBuilder sb = new StringBuilder();

            int counter = 0;

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{++counter}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.SongWriter}");

                foreach (var performer in song.SongPerformers)
                {
                    sb.AppendLine($"---Performer: {performer}");
                }

                sb.AppendLine($"---AlbumProducer: {song.Producer}");
                sb.AppendLine($"---Duration: {song.Durations}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
