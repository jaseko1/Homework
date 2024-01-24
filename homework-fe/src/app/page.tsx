import Image from "next/image";
import NotificationHub from "@/components/NotificationHub";
import ControlPanel from "@/components/ControlPanel";
//import { SignalRProvider } from "@/contexts/SignalRContext";
import QueueCounter from "@/components/QueueCounter";


export default function Home() {
  return (
      // <SignalRProvider>
          <main className="flex min-h-screen flex-col items-center justify-center p-24 gap-y-10">
              <QueueCounter />
              <ControlPanel />
              <NotificationHub />


            {/* Paticka */}
            <div className="fixed bottom-0 left-0 bg-gradient-to-t from-white via-white dark:from-black dark:via-black">
                <Image
                    src="/jaseko.svg"
                    alt="Jaseko Logo"
                    className="dark:invert"
                    width={100}
                    height={24}
                    priority
                />
            </div>
          </main>
      // </SignalRProvider>
  );
}
