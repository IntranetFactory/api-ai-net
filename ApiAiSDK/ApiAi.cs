//
// API.AI .NET SDK - client-side libraries for API.AI
// =================================================
//
// Copyright (C) 2015 by Speaktoit, Inc. (https://www.speaktoit.com)
// https://www.api.ai
//
// ***********************************************************************************************************************
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
//
// ***********************************************************************************************************************

using System;
using System.Collections;
using System.Net;
using System.IO;
using ApiAiSDK.Model;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ApiAiSDK
{
    public class ApiAi : ApiAiBase
	{
		private readonly AIConfiguration config;
		private readonly AIDataService dataService;

        // prepare an array for multiple paralell requests, but for now we just work with 1
        const int m = 1;
        private Task[] tasks = new Task[m];
        public List<AIResponse> AIResponseList { get; set; }


		public ApiAi(AIConfiguration config)
		{
			this.config = config;

			dataService = new AIDataService(this.config);
		}

		public AIResponse TextRequest(string text)
		{
			if (string.IsNullOrEmpty(text)) {
				throw new ArgumentNullException("text");
			}

			return TextRequest(new AIRequest(text));
		}

		public AIResponse TextRequest(AIRequest request)
		{
			if (request == null) {
				throw new ArgumentNullException("request");
			}

			return dataService.Request(request);
		}


        public void TextRequestStart(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            TextRequestStart(new AIRequest(text));

            return;
        }


        public void TextRequestStart(AIRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            AIResponseList = new List<AIResponse>();
            for (var i = 0; i < m; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => TextRequestExecAsync(request));
            }
        }

        private void TextRequestExecAsync(AIRequest request) {
            var response = dataService.Request(request);
            AIResponseList.Add(response);
        }

        public AIResponse TextRequestFinish() {
            Task.WaitAll(tasks);

            if (AIResponseList.Count == 0) return null;

            return AIResponseList[0];
        }

        public AIResponse TextRequest(string text, RequestExtras requestExtras)
        {
            if (string.IsNullOrEmpty(text)) {
                throw new ArgumentNullException("text");
            }

            return TextRequest(new AIRequest(text, requestExtras));
        }

		public AIResponse VoiceRequest(Stream voiceStream, RequestExtras requestExtras = null)
		{
		    if (config.Language == SupportedLanguage.Italian)
		    {
		        throw new AIServiceException("Sorry, but Italian language now is not supported in Speaktoit recognition. Please use some another speech recognition engine.");
		    }

			return dataService.VoiceRequest(voiceStream, requestExtras);
		}

		public AIResponse VoiceRequest(float[] samples)
		{
			try {

				var trimmedSamples = TrimSilence(samples);
			
				if (trimmedSamples != null) {
				
					var pcm16 = ConvertIeeeToPcm16(trimmedSamples);
					var bytes = ConvertArrayShortToBytes(pcm16);

					var voiceStream = new MemoryStream(bytes);
					voiceStream.Seek(0, SeekOrigin.Begin);
				
					var aiResponse = VoiceRequest(voiceStream);
					return aiResponse;
				}

			} catch (AIServiceException) {
				throw;
			} catch (Exception e) {
				throw new AIServiceException(e);
			}

			return null;
		}

		
	}
}