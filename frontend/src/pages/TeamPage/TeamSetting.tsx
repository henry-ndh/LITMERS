import { useState } from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

import TeamDetails from './components/TeamDetails';
import TeamMembers from './components/TeamMembers';
import TeamInvites from './components/TeamInvites';
import TeamActivity from './components/TeamActivity';
import TeamProjects from './components/TeamProjects';
import { Users, UserPlus, Activity, Settings, Folder } from 'lucide-react';

interface Team {
  id: string;
  name: string;
  owner_id: string;
  created_at: string;
  updated_at: string;
}

interface TeamSettingsProps {
  team: Team;
  currentUserId: string;
}

export default function TeamSetting({
  team,
  currentUserId
}: TeamSettingsProps) {
  const [activeTab, setActiveTab] = useState('details');

  if (!team || !team.id) {
    return (
      <div className="container mx-auto max-w-6xl px-4 py-8">
        <div className="text-center text-muted-foreground">
          Team information not found
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto max-w-6xl px-4 py-8">
      <div className="mb-8">
        <h1 className="mb-2 text-3xl font-bold tracking-tight">
          {team.name || 'Unnamed Team'}
        </h1>
        <p className="text-muted-foreground">
          Manage team settings, members, and activities
        </p>
      </div>

      <Tabs
        value={activeTab}
        onValueChange={setActiveTab}
        className="space-y-6"
      >
        <TabsList className="grid w-full grid-cols-5 lg:inline-grid lg:w-auto">
          <TabsTrigger value="details" className="flex items-center gap-2">
            <Settings className="h-4 w-4" />
            <span className="hidden sm:inline">Details</span>
          </TabsTrigger>
          <TabsTrigger value="projects" className="flex items-center gap-2">
            <Folder className="h-4 w-4" />
            <span className="hidden sm:inline">Projects</span>
          </TabsTrigger>
          <TabsTrigger value="members" className="flex items-center gap-2">
            <Users className="h-4 w-4" />
            <span className="hidden sm:inline">Members</span>
          </TabsTrigger>
          <TabsTrigger value="invites" className="flex items-center gap-2">
            <UserPlus className="h-4 w-4" />
            <span className="hidden sm:inline">Invitations</span>
          </TabsTrigger>
          <TabsTrigger value="activity" className="flex items-center gap-2">
            <Activity className="h-4 w-4" />
            <span className="hidden sm:inline">Activity</span>
          </TabsTrigger>
        </TabsList>

        <TabsContent value="details" className="space-y-6">
          <TeamDetails team={team} currentUserId={currentUserId} />
        </TabsContent>

        <TabsContent value="projects" className="space-y-6">
          <TeamProjects teamId={team.id} currentUserId={currentUserId} />
        </TabsContent>

        <TabsContent value="members" className="space-y-6">
          <TeamMembers teamId={team.id} currentUserId={currentUserId} />
        </TabsContent>

        <TabsContent value="invites" className="space-y-6">
          <TeamInvites teamId={team.id} currentUserId={currentUserId} />
        </TabsContent>

        <TabsContent value="activity" className="space-y-6">
          <TeamActivity teamId={team.id} />
        </TabsContent>
      </Tabs>
    </div>
  );
}
