
// <copyright file="PolicyBuilder.cs" company="NA">
//NA
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DemoAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainProgram.Run<Startup>(args);
        }
    }
}
