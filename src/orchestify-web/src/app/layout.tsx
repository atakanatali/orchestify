import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { Providers } from "./providers";
import { Sidebar } from "@/components/layout/Sidebar";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Orchestify",
  description: "AI-powered code orchestration platform",
  icons: {
    icon: "/logo_v2.png",
    apple: "/logo_v2.png",
  },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <Providers>
          <div className="flex h-screen bg-[#F0F2F5] overflow-hidden">
            <Sidebar />
            <main className="flex-1 flex flex-col p-6 pl-4 transition-all duration-300">
              <div className="flex-1 overflow-auto bg-white rounded-[32px] shadow-[0px_10px_40px_rgba(0,0,0,0.08)] border border-slate-200/50 flex flex-col overflow-hidden relative">
                {/* Minimal Top Shadow Indicator */}
                <div className="absolute top-0 left-0 right-0 h-4 bg-gradient-to-b from-slate-500/5 to-transparent pointer-events-none z-10" />

                <div className="flex-1 overflow-auto">
                  {children}
                </div>
              </div>
            </main>
          </div>
        </Providers>
      </body>
    </html>
  );
}
