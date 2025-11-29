import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/components/ui/dialog';
import { Alert, AlertDescription } from '@/components/ui/alert';
import {
  Users,
  Plus,
  Crown,
  Shield,
  User,
  Search,
  ArrowRight,
  AlertTriangle
} from 'lucide-react';
import { useGetMyTeams, useCreateTeam } from '@/queries/team.query';
import { toast } from 'sonner';

type TeamRole = 'OWNER' | 'ADMIN' | 'MEMBER';

interface Team {
  id: string;
  name: string;
  owner_id: string;
  created_at: string;
  updated_at: string;
  member_count: number;
  project_count: number;
  my_role: TeamRole;
  owner: {
    name: string;
    email: string;
    profile_image?: string;
  };
}

interface TeamListProps {
  userId: string;
}

const ROLE_CONFIG = {
  OWNER: {
    label: 'Owner',
    icon: Crown,
    color: 'text-yellow-600',
    bgColor: 'bg-yellow-50 text-yellow-700 border-yellow-200'
  },
  ADMIN: {
    label: 'Admin',
    icon: Shield,
    color: 'text-blue-600',
    bgColor: 'bg-blue-50 text-blue-700 border-blue-200'
  },
  MEMBER: {
    label: 'Member',
    icon: User,
    color: 'text-gray-600',
    bgColor: 'bg-gray-50 text-gray-700 border-gray-200'
  }
};

export default function TeamList({ userId }: TeamListProps) {
  const navigate = useNavigate();
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [teamName, setTeamName] = useState('');
  const [error, setError] = useState('');
  const [searchQuery, setSearchQuery] = useState('');

  const { data: teamsData, isLoading } = useGetMyTeams();
  const createTeamMutation = useCreateTeam();

  const handleSelectTeam = (teamId: string) => {
    navigate(`/team/${teamId}`);
  };

  const teams: Team[] = teamsData?.data || teamsData || [];

  const handleCreateTeam = async () => {
    if (!teamName.trim()) {
      setError('Please enter a team name');
      return;
    }

    setError('');

    try {
      await createTeamMutation.mutateAsync({ name: teamName });
      toast.success('Team created successfully');
      setTeamName('');
      setIsCreateDialogOpen(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create team');
      toast.error('Failed to create team');
    }
  };

  const filteredTeams = teams.filter((team) =>
    team?.name?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const getInitials = (name?: string) => {
    if (!name) return '??';
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <div className="container mx-auto max-w-6xl px-4 py-8">
      <div className="mb-8">
        <div className="mb-4 flex items-center justify-between">
          <div>
            <h1 className="mb-2 text-3xl font-bold tracking-tight">Teams</h1>
            <p className="text-muted-foreground">
              Manage teams you're part of
            </p>
          </div>

          <Dialog
            open={isCreateDialogOpen}
            onOpenChange={setIsCreateDialogOpen}
          >
            <DialogTrigger asChild>
              <Button className="gap-2">
                <Plus className="h-4 w-4" />
                Create Team
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Create New Team</DialogTitle>
                <DialogDescription>
                  Create a new team to collaborate with other members
                </DialogDescription>
              </DialogHeader>

              <div className="space-y-4 py-4">
                {error && (
                  <Alert variant="destructive">
                    <AlertTriangle className="h-4 w-4" />
                    <AlertDescription>{error}</AlertDescription>
                  </Alert>
                )}

                <div className="space-y-2">
                  <Label htmlFor="teamName">Team Name</Label>
                  <Input
                    id="teamName"
                    placeholder="e.g., Marketing Team"
                    value={teamName}
                    onChange={(e) => setTeamName(e.target.value)}
                    disabled={createTeamMutation.isPending}
                    maxLength={50}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        handleCreateTeam();
                      }
                    }}
                  />
                  <p className="text-xs text-muted-foreground">
                    {teamName.length}/50 characters
                  </p>
                </div>
              </div>

              <DialogFooter>
                <Button
                  variant="outline"
                  onClick={() => {
                    setIsCreateDialogOpen(false);
                    setTeamName('');
                    setError('');
                  }}
                  disabled={createTeamMutation.isPending}
                >
                  Cancel
                </Button>
                <Button
                  onClick={handleCreateTeam}
                  disabled={createTeamMutation.isPending}
                >
                  {createTeamMutation.isPending ? 'Creating...' : 'Create Team'}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>

        <div className="relative">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 transform text-muted-foreground" />
          <Input
            placeholder="Search teams..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="max-w-md pl-10"
          />
        </div>
      </div>

      {isLoading ? (
        <Card>
          <CardContent className="p-12">
            <div className="text-center text-muted-foreground">Loading...</div>
          </CardContent>
        </Card>
      ) : filteredTeams.length === 0 ? (
        <Card>
          <CardContent className="p-12">
            <div className="text-center">
              <Users className="mx-auto mb-4 h-16 w-16 text-muted-foreground" />
              <h3 className="mb-2 text-lg font-semibold">
                {searchQuery ? 'No teams found' : 'No teams yet'}
              </h3>
              <p className="mb-6 text-muted-foreground">
                {searchQuery
                  ? 'Try searching with different keywords'
                  : 'Create your first team to start collaborating with your teammates'}
              </p>
              {!searchQuery && (
                <Button
                  onClick={() => setIsCreateDialogOpen(true)}
                  className="gap-2"
                >
                  <Plus className="h-4 w-4" />
                  Create Your First Team
                </Button>
              )}
            </div>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
          {filteredTeams.map((team) => {
            if (!team) return null;

            const RoleIcon = ROLE_CONFIG[team.my_role]?.icon || User;

            return (
              <Card
                key={team.id}
                className="group cursor-pointer transition-shadow hover:shadow-md"
                onClick={() => handleSelectTeam(team.id)}
              >
                <CardHeader className="space-y-4">
                  <div className="flex items-start justify-between">
                    <div className="flex items-center gap-3">
                      <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                        <Users className="h-6 w-6 text-primary" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <h3 className="truncate text-lg font-semibold transition-colors group-hover:text-primary">
                          {team.name || 'Unnamed Team'}
                        </h3>
                      </div>
                    </div>
                    <ArrowRight className="h-5 w-5 text-muted-foreground transition-all group-hover:translate-x-1 group-hover:text-primary" />
                  </div>

                  <Badge
                    variant="outline"
                    className={`w-fit ${ROLE_CONFIG[team.my_role]?.bgColor || 'bg-gray-50'}`}
                  >
                    <RoleIcon className="mr-1 h-3 w-3" />
                    {ROLE_CONFIG[team.my_role]?.label || 'Member'}
                  </Badge>
                </CardHeader>

                <CardContent className="space-y-4">
                  <div className="flex items-center gap-4 text-sm text-muted-foreground">
                    <div className="flex items-center gap-1">
                      <Users className="h-4 w-4" />
                      <span>{team.member_count || 0} thành viên</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <span>•</span>
                      <span>{team.project_count || 0} project</span>
                    </div>
                  </div>

                  {team.owner && (
                    <div className="flex items-center gap-2 border-t pt-2">
                      <Avatar className="h-6 w-6">
                        <AvatarImage src={team.owner?.profile_image} />
                        <AvatarFallback className="bg-muted text-xs">
                          {getInitials(team.owner?.name)}
                        </AvatarFallback>
                      </Avatar>
                      <div className="min-w-0 flex-1">
                        <p className="truncate text-xs text-muted-foreground">
                          Owner: {team.owner?.name || 'N/A'}
                        </p>
                      </div>
                    </div>
                  )}
                </CardContent>
              </Card>
            );
          })}
        </div>
      )}
    </div>
  );
}
