using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net.Http;
using System.Buffers;
using System.IO;

namespace sampleDataApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {

        private readonly ILogger<DemoController> _logger;
        private readonly IConfiguration _configuration;

        private static string filePath = "./wwwroot/images/cover.jpg";

        public DemoController(IConfiguration configuration, ILogger<DemoController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("sample1")]
        public IEnumerable<string> Get()
        {
            string connectionString = _configuration.GetConnectionString("MusicStore");
            var result = new List<string>();
            using (var conn = new SqlConnection(connectionString))
            {
                var comm = new SqlCommand("SELECT ArtistId, Name FROM [dbo].[Artists] WHERE ArtistId<=20", conn);
                var comm2 = new SqlCommand("SELECT AlbumId, Title, AlbumArtUrl FROM [dbo].[Albums] WHERE ArtistId = @ArtistId", conn);
                comm2.Parameters.Add("@ArtistId", SqlDbType.Int);
                conn.Open();
                using (var artistsReader = comm.ExecuteReader())
                {
                    while (artistsReader.Read())
                    {
                        comm2.Parameters["@ArtistId"].Value = artistsReader.GetInt32(0);
                        using (var albumsReader = comm2.ExecuteReader())
                        {
                            while (albumsReader.Read())
                            {

                                var cover = System.IO.File.ReadAllBytes(filePath);

                                result.Add($"{artistsReader.GetString(1)}:{albumsReader.GetString(1)}");
                            }
                        }

                    }
                }
            }
            return result;
        }

        [HttpGet("sample2")]
        public async Task<IEnumerable<string>> GetV2()
        {
            string connectionString = _configuration.GetConnectionString("MusicStore");
            var result = new List<string>();

            using (var conn = new SqlConnection(connectionString))
            {
                var comm = new SqlCommand("SELECT ArtistId, Name FROM [dbo].[Artists] WHERE ArtistId<=20", conn);
                var comm2 = new SqlCommand("SELECT AlbumId, Title, AlbumArtUrl FROM [dbo].[Albums] WHERE ArtistId = @ArtistId", conn);
                comm2.Parameters.Add("@ArtistId", SqlDbType.Int);
                await conn.OpenAsync();
                using (var artistsReader = await comm.ExecuteReaderAsync())
                {
                    while (artistsReader.Read())
                    {
                        comm2.Parameters["@ArtistId"].Value = artistsReader.GetInt32(0);
                        using (var albumsReader = await comm2.ExecuteReaderAsync())
                        {
                            while (albumsReader.Read())
                            {
                                var cover = await System.IO.File.ReadAllBytesAsync(filePath);

                                result.Add($"{artistsReader.GetString(1)}:{albumsReader.GetString(1)}");
                            }
                        }

                    }
                }
            }
            return result;
        }

        [HttpGet("sample3")]
        public async Task<IEnumerable<string>> GetV3()
        {
            string connectionString = _configuration.GetConnectionString("MusicStore");
            var result = new List<string>();

            using (var conn = new SqlConnection(connectionString))
            {
                var comm = new SqlCommand("SELECT ArtistId, Name FROM [dbo].[Artists] WHERE ArtistId<=20", conn);
                var comm2 = new SqlCommand("SELECT AlbumId, Title, AlbumArtUrl FROM [dbo].[Albums] WHERE ArtistId = @ArtistId", conn);
                comm2.Parameters.Add("@ArtistId", SqlDbType.Int);
                await conn.OpenAsync();
                using (var artistsReader = await comm.ExecuteReaderAsync())
                {
                    while (artistsReader.Read())
                    {
                        comm2.Parameters["@ArtistId"].Value = artistsReader.GetInt32(0);
                        using (var albumsReader = await comm2.ExecuteReaderAsync())
                        {
                            while (albumsReader.Read())
                            {
                                var cover = ArrayPool<byte>.Shared.Rent(196590);

                                try
                                {
                                    using (var fileStream = System.IO.File.OpenRead(filePath))
                                    {
                                        using (var memStream = new MemoryStream(cover))
                                        {
                                            await fileStream.CopyToAsync(memStream);
                                        }
                                    }
                                }
                                finally
                                {
                                    ArrayPool<byte>.Shared.Return(cover, false);
                                }

                                result.Add($"{artistsReader.GetString(1)}:{albumsReader.GetString(1)}");
                            }
                        }

                    }
                }
            }
            return result;
        }

        [HttpGet("sample4")]
        public async Task<IEnumerable<string>> GetV4()
        {
            string connectionString = _configuration.GetConnectionString("MusicStore");
            var result = new List<string>();

            using (var conn = new SqlConnection(connectionString))
            {
                var comm = new SqlCommand(@"SELECT a.ArtistId, Name, AlbumId, Title
                                            FROM [dbo].[Artists] a
                                            INNER JOIN [dbo].[Albums] b ON a.ArtistId = b.ArtistId
                                            WHERE a.ArtistId<=20", conn);
                await conn.OpenAsync();
                using (var artistsReader = await comm.ExecuteReaderAsync())
                {
                    while (artistsReader.Read())
                    {
                        var cover = ArrayPool<byte>.Shared.Rent(196590);
                        try
                        {
                            using (var fileStream = System.IO.File.OpenRead(filePath))
                            {
                                using (var memStream = new MemoryStream(cover))
                                {
                                    await fileStream.CopyToAsync(memStream);
                                }
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(cover, false);
                        }

                        result.Add($"{artistsReader.GetString(1)}:{artistsReader.GetString(3)}");
                    }
                }

            }
            return result;
        }
    }
}
