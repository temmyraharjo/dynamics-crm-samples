<template>
  <div>
    <div v-if="getIsLogin()">
      <h1>Welcome {{ getUserName() }}</h1>
      <button @click="doLogOut()">Logout</button>
    </div>
    <button v-if="!getIsLogin()" @click="doLogin()">Login</button>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import {
  Configuration,
  AuthenticationResult,
  PublicClientApplication,
} from "@azure/msal-browser";
import store from "@/store";

@Component
export default class Login extends Vue {
  getIsLogin(): boolean {
    return store.state.isLogin;
  }

  getUserName(): string {
    return store.state.preferredName;
  }

  getMsalConfig(): Configuration {
    const msalConfig: Configuration = {
      auth: {
        clientId: "5a9ffc06-fec4-4fee-b41d-a299585f23cb",
        redirectUri: "http://localhost:8080/",
        authority:
          "https://login.microsoftonline.com/insurgodevu.onmicrosoft.com",
      },
      cache: {
        cacheLocation: "sessionStorage",
        storeAuthStateInCookie: true,
      },
    };

    return msalConfig;
  }

  doLogin(): void {
    if (store.getters.isLogin) return;

    const config = this.getMsalConfig();
    const userAgentApp = new PublicClientApplication(config);
    userAgentApp
      .loginPopup({
        scopes: ["user.read"],
        prompt: "select_account",
      })
      .then((value: AuthenticationResult) => {
        store.commit("doLogin", value);
        const account = userAgentApp.getAccountByHomeId(
          store.state.homeAccountId
        );
        return userAgentApp.acquireTokenSilent({
          scopes: ["https://org2c8fdee0.crm5.dynamics.com/.default"],
          account: account || undefined,
        });
      })
      .then((value: AuthenticationResult) => {
        store.commit("getAccessToken", value);
      });
  }

  doLogOut(): void {
    const config = this.getMsalConfig();
    const userAgentApp = new PublicClientApplication(config);
    userAgentApp.logoutPopup({
      mainWindowRedirectUri: "http://localhost:8080/",
      account: userAgentApp.getAccountByHomeId(store.state.homeAccountId),
    });
  }
}
</script>
