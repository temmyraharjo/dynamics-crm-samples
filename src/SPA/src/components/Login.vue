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
        clientId: "your_client_id",
        redirectUri: "http://localhost:8080/",
        authority:
          "https://login.microsoftonline.com/your_ad_domain.onmicrosoft.com",
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
          scopes: ["https://your_crm_org.crm5.dynamics.com/.default"],
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
