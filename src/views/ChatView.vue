<script setup lang="ts">
import { ref, nextTick } from "vue";
import { useI18n } from "vue-i18n";
import ChatMessage from "../components/Chat/ChatMessage.vue";
import ChatInput from "../components/Chat/ChatInput.vue";
import { useUserStore } from "../stores/user";
import { Flower2 } from "lucide-vue-next";

const API = import.meta.env.VITE_API_URL || "http://localhost:5000";

interface Message {
  role: "user" | "assistant";
  content: string;
  time: string;
}

const { t } = useI18n();
const user = useUserStore();
const loading = ref(false);
const scrollContainer = ref<HTMLElement | null>(null);

const messages = ref<Message[]>([
  {
    role: "assistant",
    content: user.displayName
      ? t("chat.greeting", { name: user.displayName })
      : t("chat.greetingNoName"),
    time: formatTime(new Date()),
  },
]);

function formatTime(d: Date) {
  return d.toLocaleTimeString("en-US", {
    hour: "numeric",
    minute: "2-digit",
    hour12: true,
  });
}

async function handleSend(text: string) {
  if (loading.value) return;

  messages.value.push({
    role: "user",
    content: text,
    time: formatTime(new Date()),
  });
  loading.value = true;
  await scrollToBottom();

  try {
    const history = messages.value
      .slice(0, -1)
      .map(({ role, content }) => ({ role, content }));

    const res = await fetch(`${API}/api/chat`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${user.token}`,
      },
      body: JSON.stringify({ message: text, history }),
    });

    if (res.ok) {
      const data = await res.json();
      messages.value.push({
        role: "assistant",
        content: data.reply,
        time: formatTime(new Date()),
      });
    } else {
      messages.value.push({
        role: "assistant",
        content: t("chat.errorReply"),
        time: formatTime(new Date()),
      });
    }
  } catch {
    messages.value.push({
      role: "assistant",
      content: t("chat.errorServer"),
      time: formatTime(new Date()),
    });
  } finally {
    loading.value = false;
    await scrollToBottom();
  }
}

async function scrollToBottom() {
  await nextTick();
  if (scrollContainer.value) {
    scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
  }
}
</script>

<template>
  <div class="flex-1 min-h-0 flex flex-col">
    <div
      class="flex-1 flex flex-col bg-transparent max-w-[900px] mx-auto w-full min-h-0"
    >
      <div
        ref="scrollContainer"
        class="flex-1 overflow-y-auto px-4 pb-4 pt-6 scrollbar-none"
      >
        <ChatMessage
          v-for="(msg, i) in messages"
          :key="i"
          :isAssistant="msg.role === 'assistant'"
          :time="msg.time"
          :content="msg.content"
        >
          {{ msg.content }}
        </ChatMessage>

        <div v-if="loading" class="flex gap-3 w-full mb-4">
          <div class="shrink-0 flex items-start">
            <div
              class="w-8 h-8 rounded-full flex items-center justify-center mt-[1.4rem] bg-light-green-bg text-success-green"
            >
              <Flower2 width="16" height="16" />
            </div>
          </div>
          <div class="flex flex-col max-w-[80%] max-sm:max-w-[90%]">
            <span
              class="text-[0.65rem] font-extrabold text-text-muted tracking-[0.5px] mb-2 uppercase"
              >{{ t('chat.assistantLabel') }}</span
            >
            <div
              class="p-4 max-sm:p-3 rounded-lg bg-surface border border-border rounded-tl-[4px] flex gap-1.5 items-center"
            >
              <span
                class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:0ms]"
              ></span>
              <span
                class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:150ms]"
              ></span>
              <span
                class="w-2 h-2 bg-text-muted rounded-full animate-bounce [animation-delay:300ms]"
              ></span>
            </div>
          </div>
        </div>
      </div>

      <div class="px-4 pb-4 pt-2">
        <ChatInput :disabled="loading" @send="handleSend" />
      </div>
    </div>
  </div>
</template>
