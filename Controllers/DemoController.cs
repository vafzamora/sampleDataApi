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

namespace sampleDataApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {

        private readonly ILogger<DemoController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private  Uri _imageUri; 

        public DemoController(IConfiguration configuration, ILogger<DemoController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;            
        }

        [HttpGet("sample1")]
        public async Task<IEnumerable<string>> Get()
        {
             _imageUri =  new  Uri(new Uri($"{Request.Scheme}://{Request.Host}"),"/images/cover.jpg");
            string connectionString = _configuration.GetConnectionString("MusicStore");
            var result = new List<string>();
            var httpClient = _httpClientFactory.CreateClient();
            using(var conn = new SqlConnection(connectionString)){
                var comm = new SqlCommand("SELECT TOP 20 ArtistId, Name FROM [dbo].[Artists]",conn);
                var comm2 = new SqlCommand("SELECT AlbumId, Title, AlbumArtUrl FROM [dbo].[Albums] WHERE ArtistId = @ArtistId",conn);
                comm2.Parameters.Add("@ArtistId",SqlDbType.Int);
                conn.Open();
                using(var artistsReader = comm.ExecuteReader()){
                while (artistsReader.Read())
                {
                    comm2.Parameters["@ArtistId"].Value = artistsReader.GetInt32(0);
                    using (var albumsReader = comm2.ExecuteReader()){
                    while (albumsReader.Read())
                    {
                        
                        var cover =  await httpClient.GetByteArrayAsync(_imageUri); 

                        result.Add($"{artistsReader.GetString(1)}:{albumsReader.GetString(1)}");
                    }}

                }}
            }
            return result;
        }
    

        [HttpGet("sample2")]
        public async Task<IEnumerable<string>> GetV2()
        {
            string connectionString = _configuration.GetConnectionString("MusicStore");
            var result = new List<string>();
            using(var conn = new SqlConnection(connectionString)){
                var comm = new SqlCommand("SELECT TOP 20 ArtistId, Name FROM [dbo].[Artists]",conn);
                var comm2 = new SqlCommand("SELECT AlbumId, Title, AlbumArtUrl FROM [dbo].[Albums] WHERE ArtistId = @ArtistId",conn);
                comm2.Parameters.Add("@ArtistId",SqlDbType.Int);
                await conn.OpenAsync();
                using(var artistsReader = await comm.ExecuteReaderAsync()){
                while (artistsReader.Read())
                {
                    comm2.Parameters["@ArtistId"].Value = artistsReader.GetInt32(0);
                    using (var albumsReader = await comm2.ExecuteReaderAsync()){
                    while (albumsReader.Read())
                    {
                        result.Add($"{artistsReader.GetString(1)}:{albumsReader.GetString(1)}");
                    }}

                }}
            }
            return result;
        }
    

    }
}
