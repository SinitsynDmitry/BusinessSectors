const { env } = require('process');

// Determine the target API URL
const target = env.API_URL ? `${env.API_URL}` :
  env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] :
    'https://localhost:7173';

console.log(`Proxying API requests to: ${target}`);

const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
      "/api/sectors",
      // Add other API paths here
    ],
    target,
    secure: false,
    logLevel: "debug"
  }
]

module.exports = PROXY_CONFIG;
