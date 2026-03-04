<script lang="ts">
  import type { PageProps } from "./$types";
  let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-4">
  <div class="flex justify-between">
    <h1 class="text-2xl font-bold text-slate-900">Users</h1>
  </div>
  <hr />

  {#if data.me}
    <div class="flex flex-col gap-2">
      <div><span class="font-semibold">Id:</span> {data.me.id}</div>
      <div><span class="font-semibold">Email:</span> {data.me.email}</div>
      <div><span class="font-semibold">Username:</span> {data.me.username}</div>
      <div><span class="font-semibold">Last Login:</span> {data.me.last_login}</div>
    </div>
  {:else}
    <p>No user data available</p>
  {/if}

</div>


