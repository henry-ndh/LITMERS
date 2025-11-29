import { useState } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Users,
  UserPlus,
  UserMinus,
  Shield,
  FolderPlus,
  FolderEdit,
  Archive,
  ArchiveRestore,
  Trash2,
  Activity as ActivityIcon,
  ChevronDown,
} from 'lucide-react';
import { useGetActivityLogs } from '@/queries/team.query';

type ActivityActionType =
  | 'TEAM_CREATED'
  | 'TEAM_UPDATED'
  | 'TEAM_DELETED'
  | 'MEMBER_INVITED'
  | 'MEMBER_JOINED'
  | 'MEMBER_KICKED'
  | 'MEMBER_LEFT'
  | 'ROLE_CHANGED'
  | 'PROJECT_CREATED'
  | 'PROJECT_UPDATED'
  | 'PROJECT_ARCHIVED'
  | 'PROJECT_UNARCHIVED'
  | 'PROJECT_DELETED';

interface ActivityLog {
  id: number;
  teamId: number;
  actorId: number;
  actorName: string;
  actorAvatar: string | null;
  actionType: number; // Enum value from backend
  actionTypeName: string; // String representation
  targetId: number | null;
  targetType: string | null;
  message: string | null;
  metadata: string | null;
  createdAt: string;
}

interface TeamActivityProps {
  teamId: string;
}

const ACTIVITY_CONFIG: Record<
  ActivityActionType,
  {
    icon: React.ElementType;
    color: string;
    bgColor: string;
    label: string;
  }
> = {
  TEAM_CREATED: {
    icon: Users,
    color: 'text-blue-600',
    bgColor: 'bg-blue-50',
    label: 'Created team',
  },
  TEAM_UPDATED: {
    icon: Users,
    color: 'text-blue-600',
    bgColor: 'bg-blue-50',
    label: 'Updated team',
  },
  TEAM_DELETED: {
    icon: Trash2,
    color: 'text-red-600',
    bgColor: 'bg-red-50',
    label: 'Deleted team',
  },
  MEMBER_INVITED: {
    icon: UserPlus,
    color: 'text-green-600',
    bgColor: 'bg-green-50',
    label: 'Invited member',
  },
  MEMBER_JOINED: {
    icon: UserPlus,
    color: 'text-green-600',
    bgColor: 'bg-green-50',
    label: 'Joined team',
  },
  MEMBER_KICKED: {
    icon: UserMinus,
    color: 'text-red-600',
    bgColor: 'bg-red-50',
    label: 'Removed member',
  },
  MEMBER_LEFT: {
    icon: UserMinus,
    color: 'text-amber-600',
    bgColor: 'bg-amber-50',
    label: 'Left team',
  },
  ROLE_CHANGED: {
    icon: Shield,
    color: 'text-purple-600',
    bgColor: 'bg-purple-50',
    label: 'Changed role',
  },
  PROJECT_CREATED: {
    icon: FolderPlus,
    color: 'text-blue-600',
    bgColor: 'bg-blue-50',
    label: 'Created project',
  },
  PROJECT_UPDATED: {
    icon: FolderEdit,
    color: 'text-blue-600',
    bgColor: 'bg-blue-50',
    label: 'Updated project',
  },
  PROJECT_ARCHIVED: {
    icon: Archive,
    color: 'text-gray-600',
    bgColor: 'bg-gray-50',
    label: 'Archived project',
  },
  PROJECT_UNARCHIVED: {
    icon: ArchiveRestore,
    color: 'text-green-600',
    bgColor: 'bg-green-50',
    label: 'Unarchived project',
  },
  PROJECT_DELETED: {
    icon: Trash2,
    color: 'text-red-600',
    bgColor: 'bg-red-50',
    label: 'Deleted project',
  },
};

export default function TeamActivity({ teamId }: TeamActivityProps) {
  const [filter, setFilter] = useState<'all' | ActivityActionType>('all');
  const [displayCount, setDisplayCount] = useState(20);

  const { data: activitiesData, isLoading } = useGetActivityLogs(teamId, displayCount);
  const activities: ActivityLog[] = (activitiesData?.data || activitiesData || [])
    .filter((activity: any) => activity && activity.actorName);

  const filteredActivities = filter === 'all'
    ? activities
    : activities.filter(a => a?.actionTypeName === filter);

  const displayedActivities = filteredActivities.slice(0, displayCount);

  // Helper function to safely get activity config
  const getActivityConfig = (actionTypeName: string) => {
    const type = actionTypeName as ActivityActionType;
    return ACTIVITY_CONFIG[type] || ACTIVITY_CONFIG.MEMBER_INVITED;
  };

  const getActivityMessage = (activity: ActivityLog): string => {
    if (activity.message) return activity.message;

    const actionTypeName = activity.actionTypeName as ActivityActionType;
    const config = getActivityConfig(activity.actionTypeName);
    const actorName = activity.actorName || 'User';
    
    // Parse metadata if it's a string
    let metadata: Record<string, any> | null = null;
    if (activity.metadata) {
      try {
        metadata = typeof activity.metadata === 'string' 
          ? JSON.parse(activity.metadata) 
          : activity.metadata;
      } catch (e) {
        metadata = null;
      }
    }

    switch (actionTypeName) {
      case 'TEAM_CREATED':
        return `${actorName} created the team`;
      case 'TEAM_UPDATED':
        return `${actorName} updated team information`;
      case 'MEMBER_INVITED':
        return `${actorName} invited ${metadata?.email || 'a member'} to the team`;
      case 'MEMBER_JOINED':
        return `${actorName} joined the team`;
      case 'MEMBER_KICKED':
        return `${actorName} removed ${metadata?.user_name || 'a member'} from the team`;
      case 'MEMBER_LEFT':
        return `${actorName} left the team`;
      case 'ROLE_CHANGED':
        return `${actorName} changed ${metadata?.user_name || 'a member'}'s role from ${metadata?.old_role || 'N/A'} to ${metadata?.new_role || 'N/A'}`;
      case 'PROJECT_CREATED':
        return `${actorName} created project "${metadata?.project_name || 'a project'}"`;
      case 'PROJECT_UPDATED':
        return `${actorName} updated project "${metadata?.project_name || 'a project'}"`;
      case 'PROJECT_ARCHIVED':
        return `${actorName} archived project "${metadata?.project_name || 'a project'}"`;
      case 'PROJECT_UNARCHIVED':
        return `${actorName} unarchived project "${metadata?.project_name || 'a project'}"`;
      case 'PROJECT_DELETED':
        return `${actorName} deleted project "${metadata?.project_name || 'a project'}"`;
      default:
        return `${actorName} performed ${config.label.toLowerCase()}`;
    }
  };

  const getTimeAgo = (dateString: string): string => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (diffInSeconds < 60) return 'Just now';
    if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} minute${Math.floor(diffInSeconds / 60) > 1 ? 's' : ''} ago`;
    if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} hour${Math.floor(diffInSeconds / 3600) > 1 ? 's' : ''} ago`;
    if (diffInSeconds < 604800) return `${Math.floor(diffInSeconds / 86400)} day${Math.floor(diffInSeconds / 86400) > 1 ? 's' : ''} ago`;
    
    return date.toLocaleDateString('en-US', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const groupActivitiesByDate = (activities: ActivityLog[]) => {
    const groups: Record<string, ActivityLog[]> = {};
    
    activities.forEach(activity => {
      const date = new Date(activity.createdAt);
      const today = new Date();
      const yesterday = new Date(today);
      yesterday.setDate(yesterday.getDate() - 1);
      
      let key: string;
      if (date.toDateString() === today.toDateString()) {
        key = 'Today';
      } else if (date.toDateString() === yesterday.toDateString()) {
        key = 'Yesterday';
      } else {
        key = date.toLocaleDateString('en-US', {
          day: '2-digit',
          month: 'long',
          year: 'numeric',
        });
      }
      
      if (!groups[key]) groups[key] = [];
      groups[key].push(activity);
    });
    
    return groups;
  };

  const groupedActivities = groupActivitiesByDate(displayedActivities);

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div>
            <CardTitle>Team Activity</CardTitle>
            <CardDescription>
              Track all activities in your team
            </CardDescription>
          </div>
          
          <Select value={filter} onValueChange={(value) => setFilter(value as any)}>
            <SelectTrigger className="w-[200px]">
              <SelectValue placeholder="Filter activities" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All activities</SelectItem>
              <SelectItem value="MEMBER_JOINED">Member joined</SelectItem>
              <SelectItem value="MEMBER_INVITED">Invitations</SelectItem>
              <SelectItem value="ROLE_CHANGED">Role changed</SelectItem>
              <SelectItem value="PROJECT_CREATED">Project created</SelectItem>
              <SelectItem value="PROJECT_UPDATED">Project updated</SelectItem>
              <SelectItem value="PROJECT_ARCHIVED">Project archived</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <div className="text-center py-12 text-muted-foreground">
            Loading...
          </div>
        ) : filteredActivities.length === 0 ? (
          <div className="text-center py-12 border rounded-lg bg-muted/30">
            <ActivityIcon className="h-12 w-12 mx-auto text-muted-foreground mb-3" />
            <p className="text-muted-foreground">No activities yet</p>
          </div>
        ) : (
          <div className="space-y-6">
            {Object.entries(groupedActivities).map(([date, activities]) => (
              <div key={date} className="space-y-4">
                <div className="flex items-center gap-2">
                  <Badge variant="outline" className="font-normal">
                    {date}
                  </Badge>
                  <div className="flex-1 h-px bg-border" />
                </div>
                
                <div className="space-y-3">
                  {activities.map((activity) => {
                    const config = getActivityConfig(activity.actionTypeName);
                    const Icon = config.icon;

                    return (
                      <div
                        key={activity.id}
                        className="flex gap-4 p-3 rounded-lg hover:bg-muted/50 transition-colors"
                      >
                        <div className={`h-10 w-10 rounded-full ${config.bgColor} flex items-center justify-center flex-shrink-0`}>
                          <Icon className={`h-5 w-5 ${config.color}`} />
                        </div>

                        <div className="flex-1 min-w-0">
                          <div className="flex items-start justify-between gap-2">
                            <p className="text-sm">
                              {getActivityMessage(activity)}
                            </p>
                            <span className="text-xs text-muted-foreground whitespace-nowrap">
                              {getTimeAgo(activity.createdAt)}
                            </span>
                          </div>
                          
                          {activity.actorName && (
                            <div className="flex items-center gap-2 mt-1">
                              <Avatar className="h-5 w-5">
                                <AvatarImage src={activity.actorAvatar || undefined} />
                                <AvatarFallback className="text-[10px] bg-muted">
                                  {getInitials(activity.actorName)}
                                </AvatarFallback>
                              </Avatar>
                              <span className="text-xs text-muted-foreground">
                                {activity.actorName || 'N/A'}
                              </span>
                            </div>
                          )}
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            ))}

            {activities.length >= displayCount && (
              <div className="flex justify-center pt-4">
                <Button
                  variant="outline"
                  onClick={() => setDisplayCount(prev => prev + 20)}
                  className="gap-2"
                >
                  <ChevronDown className="h-4 w-4" />
                  Load more
                </Button>
              </div>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
