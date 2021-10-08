<template>
  <div class="home">
    <Login></Login>
    <Account></Account>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import Login from "@/components/Login.vue";
import Account from "@/components/Account.vue";

@Component({
  components: {
    Login,
    Account,
  },
})
export default class Home extends Vue {}
</script>
