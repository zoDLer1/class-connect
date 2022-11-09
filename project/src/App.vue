<template>
  <router-view/>
</template>

<script>


export default {
  name: 'App'
}
</script>

<style>
  @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&family=Ubuntu:wght@400;500;700&display=swap');
  *{
    margin: 0;
    padding: 0;
    font-family: 'Roboto', sans-serif;
    font-weight: 400;
    box-sizing: border-box;
  }
  .content{
    background-color: #E6F0F9;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
  }
  .page__section{
    background-color: #fff;
    width: 1445px;
    height: 800px;
    border-radius: 10px;
    display: flex;
    flex-direction: column;
    padding: 20px 10px;
  }
  body, html, #app{ 
    height: 100%;
  }
  a{
      text-decoration: none;
      color: #fff;
  }

  h1, h2, h3, h4, h5, h6{
      font-family: 'Ubuntu', sans-serif;
      
  }
</style>
