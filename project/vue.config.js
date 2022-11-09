
const { defineConfig } = require('@vue/cli-service')
const path = require('path')

module.exports = defineConfig({
  transpileDependencies: true,
  configureWebpack:{
    resolve:{
      alias:{
        '@': path.resolve(__dirname, 'src'),
        '#': path.resolve(__dirname, 'src', 'assets', 'imgs', 'icons'),
        '$': path.resolve(__dirname, 'src', 'store', 'modules'),
      }
    }
  }
  
})
