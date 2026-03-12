using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Server 
{
   private const string apiUrl = "http://unityseclab:5000/api";
   
   private readonly HttpClient client = new();

   public async Task ClaimAnswer(bool status, int time)
   {
      var model = new AnswerModel
      {
         Solved = status,
         Time = time
      };
      
      var jsonData = JsonUtility.ToJson(model);
       
      await PostAsync($"{apiUrl}/Data", jsonData);
   }

   public async Task<ResponseAnswers> GetAnswers()
   {
      var response = await GetAsync($"{apiUrl}/Data");
      
      return JsonUtility.FromJson<ResponseAnswers>(response);
   }

   public async Task<bool> SendFeedback(string text)
   {
      var data = new RequestFeedBack{ Feedback = text };
      var json = JsonUtility.ToJson(data);
      var response = await PostAsync($"{apiUrl}/Feedback", json);
      
      return JsonUtility.FromJson<ResponseFeedback>(response).exist;
   }

   public async Task<bool> IsFeedbackExist()
   {
      var response = await GetAsync($"{apiUrl}/Feedback");
      
      return JsonUtility.FromJson<ResponseFeedback>(response).exist;
   }
   
   private async Task<string> GetAsync(string url)
   {
      using var response = await client.GetAsync(url).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      
      return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
   }

   private async Task<string> PostAsync(string url, string data)
   {
      using var content = new StringContent(data, Encoding.UTF8, "application/json");
      using var response = await client.PostAsync(url, content).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      
      return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
   }
}
