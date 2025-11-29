import { useState } from 'react';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle
} from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue
} from '@/components/ui/select';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger
} from '@/components/ui/dropdown-menu';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle
} from '@/components/ui/alert-dialog';
import {
  Search,
  MoreVertical,
  Crown,
  Shield,
  User,
  UserMinus
} from 'lucide-react';
import {
  useGetTeamMembers,
  useUpdateMemberRole,
  useRemoveMember
} from '@/queries/team.query';
import { toast } from 'sonner';

type TeamRole = 'OWNER' | 'ADMIN' | 'MEMBER';

interface TeamMember {
  id: number;
  userId: number;
  name: string;
  email: string;
  avatar: string | null;
  role: TeamRole | number;
  joinedAt: string;
}

interface TeamMembersProps {
  teamId: string;
  currentUserId: string;
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

// Map role number to role string
const ROLE_MAP: Record<number, TeamRole> = {
  0: 'OWNER',
  1: 'ADMIN',
  2: 'MEMBER'
};

const ROLE_TO_NUMBER: Record<TeamRole, number> = {
  OWNER: 0,
  ADMIN: 1,
  MEMBER: 2
};

export default function TeamMembers({
  teamId,
  currentUserId
}: TeamMembersProps) {
  const [searchQuery, setSearchQuery] = useState('');
  const [memberToRemove, setMemberToRemove] = useState<TeamMember | null>(null);

  const { data: membersData, isLoading } = useGetTeamMembers(teamId);
  const updateRoleMutation = useUpdateMemberRole();
  const removeMemberMutation = useRemoveMember();

  const members: TeamMember[] = (membersData?.data || membersData || [])
    .filter((member: any) => member)
    .map((member: any) => ({
      ...member,
      role:
        typeof member.role === 'number'
          ? ROLE_MAP[member.role] || 'MEMBER'
          : member.role
    }));

  const currentMember = members.find(
    (m) => m?.userId?.toString() === currentUserId?.toString()
  );
  const canManageMembers =
    currentMember?.role === 'OWNER' || currentMember?.role === 'ADMIN';

  const filteredMembers = members.filter(
    (member) =>
      member?.name?.toLowerCase().includes(searchQuery.toLowerCase()) ||
      member?.email?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const handleRoleChange = async (memberId: string, newRole: TeamRole) => {
    try {
      await updateRoleMutation.mutateAsync({
        teamId,
        memberId,
        role: ROLE_TO_NUMBER[newRole]
      });
      toast.success('Role updated successfully');
    } catch (error: any) {
      toast.error(error?.response?.data?.message || 'Failed to update role');
    }
  };

  const handleRemoveMember = async () => {
    if (!memberToRemove) return;

    try {
      await removeMemberMutation.mutateAsync({
        teamId,
        memberId: memberToRemove.id
      });
      toast.success('Member removed successfully');
      setMemberToRemove(null);
    } catch (error: any) {
      toast.error(error?.response?.data?.message || 'Failed to remove member');
    }
  };

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <>
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Team Members</CardTitle>
              <CardDescription>
                Manage team members and their permissions
              </CardDescription>
            </div>
            <Badge variant="secondary" className="text-sm">
              {members.length} member{members.length !== 1 ? 's' : ''}
            </Badge>
          </div>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 transform text-muted-foreground" />
            <Input
              placeholder="Search members..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-10"
            />
          </div>

          <div className="divide-y rounded-lg border">
            {isLoading ? (
              <div className="p-8 text-center text-muted-foreground">
                Loading...
              </div>
            ) : filteredMembers.length === 0 ? (
              <div className="p-8 text-center text-muted-foreground">
                No members found
              </div>
            ) : (
              filteredMembers.map((member) => {
                const RoleIcon = ROLE_CONFIG[member.role].icon;
                const canChangeRole =
                  canManageMembers &&
                  member.role !== 'OWNER' &&
                  member.userId?.toString() !== currentUserId?.toString();
                const canRemove =
                  canManageMembers &&
                  member.role !== 'OWNER' &&
                  member.userId?.toString() !== currentUserId?.toString();

                return (
                  <div
                    key={member.id}
                    className="flex items-center justify-between p-4 transition-colors hover:bg-muted/50"
                  >
                    <div className="flex flex-1 items-center gap-4">
                      <Avatar className="h-10 w-10">
                        <AvatarImage src={member.avatar || undefined} />
                        <AvatarFallback className="bg-primary/10 font-medium text-primary">
                          {getInitials(member.name || 'N/A')}
                        </AvatarFallback>
                      </Avatar>

                      <div className="min-w-0 flex-1">
                        <div className="flex items-center gap-2">
                          <p className="truncate font-medium">
                            {member.name || 'N/A'}
                          </p>
                          {member.userId?.toString() ===
                            currentUserId?.toString() && (
                            <Badge variant="outline" className="text-xs">
                              You
                            </Badge>
                          )}
                        </div>
                        <p className="truncate text-sm text-muted-foreground">
                          {member.email || 'N/A'}
                        </p>
                      </div>
                    </div>

                    <div className="flex items-center gap-3">
                      {canChangeRole ? (
                        <Select
                          value={
                            typeof member.role === 'string'
                              ? member.role
                              : ROLE_MAP[member.role as number] || 'MEMBER'
                          }
                          onValueChange={(value) =>
                            handleRoleChange(
                              member.id.toString(),
                              value as TeamRole
                            )
                          }
                        >
                          <SelectTrigger className="h-9 w-[130px]">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectItem value="ADMIN">
                              <div className="flex items-center gap-2">
                                <Shield className="h-4 w-4" />
                                <span>Admin</span>
                              </div>
                            </SelectItem>
                            <SelectItem value="MEMBER">
                              <div className="flex items-center gap-2">
                                <User className="h-4 w-4" />
                                <span>Member</span>
                              </div>
                            </SelectItem>
                          </SelectContent>
                        </Select>
                      ) : (
                        <Badge
                          variant="outline"
                          className={
                            ROLE_CONFIG[member.role]?.bgColor || 'bg-gray-50'
                          }
                        >
                          <RoleIcon className="mr-1 h-3 w-3" />
                          {ROLE_CONFIG[member.role]?.label || 'Member'}
                        </Badge>
                      )}

                      {canRemove && (
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button
                              variant="ghost"
                              size="sm"
                              className="h-8 w-8 p-0"
                            >
                              <MoreVertical className="h-4 w-4" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end">
                            <DropdownMenuItem
                              onClick={() => setMemberToRemove(member)}
                              className="text-destructive focus:text-destructive"
                            >
                              <UserMinus className="mr-2 h-4 w-4" />
                              Remove from team
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      )}
                    </div>
                  </div>
                );
              })
            )}
          </div>
        </CardContent>
      </Card>

      <AlertDialog
        open={!!memberToRemove}
        onOpenChange={() => setMemberToRemove(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Remove Member from Team</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to remove{' '}
              <strong>{memberToRemove?.name || 'this member'}</strong> from the
              team? They will lose access to all projects in the team.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={removeMemberMutation.isPending}>
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleRemoveMember}
              disabled={removeMemberMutation.isPending}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {removeMemberMutation.isPending ? 'Removing...' : 'Remove Member'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
