using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Server 
{
   private const string apiUrl = "http://unityseclab:5000/api/Data";
   
   private readonly HttpClient _client = new();

   public async Task ClaimAnswer(bool status, int time)
   {
      var model = new AnswerModel
      {
         Solved = status,
         Time = time
      };
      
      var jsonData = JsonUtility.ToJson(model);
       
      await PostAsync(jsonData);
   }

   public async Task<ResponseAnswers> GetAnswers()
   {
      var response = await GetAsync();
      
      return JsonUtility.FromJson<ResponseAnswers>(response);
   }
   
   private async Task<string> GetAsync()
   {
      using var response = await _client.GetAsync(apiUrl).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      
      return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
   }

   private async Task<string> PostAsync(string data)
   {
      using var content = new StringContent(data, Encoding.UTF8, "application/json");
      using var response = await _client.PostAsync(apiUrl, content).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      
      return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
   }
}
