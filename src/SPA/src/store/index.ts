import Vue from 'vue';
import Vuex from 'vuex';
import { AuthenticationResult } from '@azure/msal-browser';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    isLogin: false,
    idTokenExpiresOn: new Date(0),
    preferredName: '',
    accessTokenExpiresOn: new Date(0),
    accessToken: '',
    homeAccountId: '',
  },
  mutations: {
    doLogin(state, item: AuthenticationResult) {
      if (!item.idToken) return;

      state.isLogin = true;
      state.idTokenExpiresOn = item.expiresOn || new Date(0);
      state.preferredName = item.account?.username || '';
      state.homeAccountId = item.account?.homeAccountId || '';
    },
    getAccessToken(state, item: AuthenticationResult) {
      if (!item.accessToken) return;

      state.accessToken = item.accessToken;
      state.accessTokenExpiresOn = item.expiresOn || new Date(0);
    },
    doLogOut(state) {
      state.isLogin = false;
      state.preferredName = '';
      state.accessToken = '';
      state.accessTokenExpiresOn = new Date(0);
      state.idTokenExpiresOn = new Date(0);
    },
  },
});
