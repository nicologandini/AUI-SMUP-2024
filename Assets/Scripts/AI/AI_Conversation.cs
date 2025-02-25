using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using OpenAI.Models;
using UnityEngine;
using Utilities.Async;

namespace SMUP.AI {
    public class AI_Conversation : MonoBehaviour
    {
        [SerializeField] private OpenAIConfiguration configuration;
        [SerializeField] [TextArea(3, 10)] private string systemPrompt = "Sei un assistente di un negozio di scarpe.";

        private OpenAIClient openAI;
        private readonly Conversation conversation = new();
        private readonly List<Tool> assistantTools = new();

            
        private void Awake()
        {
            openAI = new OpenAIClient(configuration);
            conversation.AppendMessage(new Message(Role.System, systemPrompt));
            assistantTools.Add(Tool.GetOrCreateTool(openAI.ImagesEndPoint, nameof(ImagesEndpoint.GenerateImageAsync)));
        }

        private static bool isChatPending;

        public async Task<string> SubmitChat(string text)
        {
            if (isChatPending) { return ""; }
            isChatPending = true;

            conversation.AppendMessage(new Message(Role.User, text));

            try
            {
                var request = new ChatRequest(conversation.Messages, tools: assistantTools);
                ChatResponse response = await openAI.ChatEndpoint.StreamCompletionAsync(request, resultHandler: deltaResponse =>{}, cancellationToken: destroyCancellationToken);

                conversation.AppendMessage(response.FirstChoice.Message);
                PrintMessages();
                return response;
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case TaskCanceledException:
                    case OperationCanceledException:
                        break;
                    default:
                        //print("Errore durenrte richiesta a buddy");
                        Debug.LogError(e);
                        return "Scusa ho le pile scariche, prova a richiedere piu tardi";
                }
            }
            finally
            {
                isChatPending = false;
            }

            return "";
    
        }

        private void PrintMessages() {
            print("Sent Messages:");

            foreach (var message in conversation.Messages) {
                print(message);
            }

            print("");
        }
    }
}