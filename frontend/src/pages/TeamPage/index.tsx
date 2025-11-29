import TeamList from './components/TeamList';
import __helpers from '@/helpers';

export default function TeamPage() {
  const currentUserId = __helpers.getUserId();

  return (
    <div className="min-h-screen bg-background">
      <TeamList userId={currentUserId} />
    </div>
  );
}
