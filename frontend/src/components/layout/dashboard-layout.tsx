import __helpers from '@/helpers';

export default function DashboardLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="h-full min-h-screen  ">
      <main className="w-full">{children}</main>
    </div>
  );
}
