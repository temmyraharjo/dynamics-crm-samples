<template>
  <div>
    <h2>Account Table</h2>
    <button @click="getAccounts()">Fetch Accounts</button>
    <br />
    <br />
    <table style="padding: 20px">
      <thead>
        <tr>
          <td>Fullname</td>
          <td>Website URL</td>
        </tr>
      </thead>
      <tbody v-if="getAccountExists()">
        <tr v-for="account in accounts" :key="account.AccountId">
          <td>{{ account.name }}</td>
          <td>{{ account.websiteurl }}</td>
        </tr>
      </tbody>
      <tbody v-if="!getAccountExists()">
        <tr>
          <td colspan="2">No Data Fetch..</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import store from "@/store";
import { Component, Vue } from "vue-property-decorator";

interface Account {
  accountid: string;
  name: string;
  websiteurl: string;
}

@Component
export default class Login extends Vue {
  accounts: Account[] = [];

  getAccountExists(): boolean {
    return this.accounts.length > 0;
  }

  getAccounts(): void {
    if (!store.state.accessToken) return;

    fetch(
      "https://org2c8fdee0.crm5.dynamics.com/api/data/v9.1/accounts?$select=name,websiteurl&$top=10",
      {
        method: "get",
        headers: new Headers({
          Accept: "application/json",
          "OData-MaxVersion": "4.0",
          "OData-Version": "4.0",
          Authorization: "Bearer " + store.state.accessToken,
        }),
      }
    )
      .then((fetchValue: Response) => {
        return fetchValue.json() as Promise<{ value: Account[] }>;
      })
      .then((data: { value: Account[] }) => {
        this.accounts = data.value;

        console.log(this.accounts);
      });
  }
}
</script>

<style scoped lang="scss">
table,
th,
td {
  border: 1px solid black;
}

th,
td {
  padding: 10px;
}
</style>
