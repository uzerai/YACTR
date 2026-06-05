import { defineConfig } from '@hey-api/openapi-ts';
import { loadEnv } from 'vite';

const env = loadEnv(process.env.NODE_ENV!, process.cwd(), '');

export default defineConfig({
  input: `${env.YACTR_BASE_API_URL}/swagger/v1/swagger.json`,
  output: {
    indexFile: false,
    path: 'src/lib/api/generated'
  },
  plugins: [
    {
      name: '@hey-api/typescript',
      enums: 'javascript'
    },
    {
      name: '@hey-api/client-fetch',
      runtimeConfigPath: './src/lib/api/client_config.ts',
    },
    '@hey-api/sdk',
    'zod'
  ],
  parser: {
    transforms: {
      enums: 'root'
    },
  }
});