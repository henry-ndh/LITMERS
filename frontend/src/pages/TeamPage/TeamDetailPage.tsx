import { useParams, useNavigate } from 'react-router-dom';
import TeamSetting from './TeamSetting';
import { useGetTeamDetail } from '@/queries/team.query';
import __helpers from '@/helpers';

export default function TeamDetailPage() {
  const { teamId } = useParams<{ teamId: string }>();
  const navigate = useNavigate();
  const currentUserId = __helpers.getUserId();

  const { data: teamData, isLoading } = useGetTeamDetail(teamId);

  const team = teamData?.data || teamData;

  // Handler to go back to team list
  const handleBackToList = () => {
    navigate('/team');
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="text-muted-foreground">Loading...</div>
      </div>
    );
  }

  if (!team || !team.id) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="text-center space-y-4">
          <div className="text-muted-foreground">Team not found</div>
          <button
            onClick={handleBackToList}
            className="text-sm text-primary hover:underline"
          >
            Back to team list
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Navigation breadcrumb */}
      <div className="border-b">
        <div className="container mx-auto max-w-6xl px-4 py-3">
          <button
            onClick={handleBackToList}
            className="text-sm text-muted-foreground transition-colors hover:text-foreground"
          >
            ← Back to team list
          </button>
        </div>
      </div>

      {/* Team Settings */}
      <TeamSetting team={team} currentUserId={currentUserId} />
    </div>
  );
}

