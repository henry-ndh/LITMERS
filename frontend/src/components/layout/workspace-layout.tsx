import { ReactNode, useState } from 'react';
import { Button } from '@/components/ui/button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue
} from '@/components/ui/select';

import { Separator } from '@/components/ui/separator';
import { Plus, Users, Settings, Grid3x3 } from 'lucide-react';
import { useGetMyTeams } from '@/queries/team.query';
import CreateTeamDialog from '@/pages/TeamPage/components/CreateTeamDialog';
import NotificationBell from '@/components/NotificationBell';
import { Link } from 'react-router-dom';
import { cn } from '@/lib/utils';

interface WorkspaceLayoutProps {
  children: ReactNode;
  selectedTeamId?: string | number;
  onTeamChange?: (teamId: any) => void;
}

export default function WorkspaceLayout({
  children,
  selectedTeamId,
  onTeamChange
}: WorkspaceLayoutProps) {
  const [isCreateTeamOpen, setIsCreateTeamOpen] = useState(false);
  const { data: teamsData } = useGetMyTeams();
  const teams = teamsData?.data || teamsData || [];

  const selectedTeam = teams.find(
    (t: any) => t.id.toString() === selectedTeamId?.toString()
  );

  return (
    <div className="flex h-screen flex-col bg-slate-50">
      {/* Top Bar - Jira Style */}
      <div className="border-b border-slate-200 bg-white shadow-sm">
        <div className="flex h-14 items-center justify-between px-4">
          <div className="flex items-center gap-3">
            {/* Logo/Icon */}
            <div className="flex h-8 w-8 items-center justify-center rounded bg-gradient-to-br from-blue-600 to-blue-700">
              <Grid3x3 className="h-4 w-4 text-white" />
            </div>

            {/* Team Selector */}
            <div className="flex items-center gap-2">
              <Select
                value={selectedTeamId?.toString() || ''}
                onValueChange={(value) => onTeamChange?.(Number(value))}
              >
                <SelectTrigger
                  className={cn(
                    'h-9 w-[220px] border-none bg-transparent shadow-none',
                    'rounded-md transition-colors hover:bg-slate-100 focus:ring-0'
                  )}
                >
                  <SelectValue placeholder="Select workspace">
                    {selectedTeam ? (
                      <div className="flex items-center gap-2">
                        <div className="flex h-6 w-6 items-center justify-center rounded bg-blue-100">
                          <Users className="h-3.5 w-3.5 text-blue-700" />
                        </div>
                        <span className="text-sm font-semibold text-slate-800">
                          {selectedTeam.name}
                        </span>
                      </div>
                    ) : (
                      <span className="text-slate-600">Select workspace</span>
                    )}
                  </SelectValue>
                </SelectTrigger>
                <SelectContent className="w-[280px]">
                  <div className="px-2 py-2">
                    <p className="text-xs font-semibold uppercase tracking-wider text-slate-500">
                      Your Workspaces
                    </p>
                  </div>
                  <Separator className="my-1" />
                  {teams.map((team: any) => (
                    <SelectItem
                      key={team.id}
                      value={team.id.toString()}
                      className="my-0.5 cursor-pointer"
                    >
                      <div className="flex items-center gap-2.5 py-1">
                        <div className="flex h-7 w-7 items-center justify-center rounded bg-blue-100">
                          <Users className="h-4 w-4 text-blue-700" />
                        </div>
                        <span className="font-medium text-slate-800">
                          {team.name}
                        </span>
                      </div>
                    </SelectItem>
                  ))}
                  <Separator className="my-1" />
                  <div className="p-1">
                    <Button
                      variant="ghost"
                      size="sm"
                      className="w-full justify-start gap-2 text-slate-600 hover:bg-slate-100 hover:text-slate-900"
                      onClick={() => setIsCreateTeamOpen(true)}
                    >
                      <Plus className="h-4 w-4" />
                      Create New Workspace
                    </Button>
                  </div>
                </SelectContent>
              </Select>

              <Separator orientation="vertical" className="h-6 bg-slate-300" />

              <Button
                variant="ghost"
                size="sm"
                className="h-9 gap-2 text-slate-600 hover:bg-slate-100 hover:text-slate-900"
                onClick={() => setIsCreateTeamOpen(true)}
              >
                <Plus className="h-4 w-4" />
                <span className="hidden font-medium sm:inline">Create</span>
              </Button>
            </div>
          </div>

          {/* Right side actions */}
          <div className="flex items-center gap-2">
            {selectedTeam && (
              <>
                <Separator
                  orientation="vertical"
                  className="h-6 bg-slate-300"
                />

                <NotificationBell />

                <Button
                  variant="ghost"
                  size="icon"
                  className="h-9 w-9 text-slate-600 hover:bg-slate-100 hover:text-slate-900"
                  asChild
                >
                  <Link to="/profile">
                    <Settings className="h-4 w-4" />
                  </Link>
                </Button>
              </>
            )}
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex flex-1 overflow-hidden bg-slate-50">{children}</div>

      <CreateTeamDialog
        open={isCreateTeamOpen}
        onOpenChange={setIsCreateTeamOpen}
      />
    </div>
  );
}
