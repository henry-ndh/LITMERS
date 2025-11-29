import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
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
  useGetTeamMembers,
  useGetTeamDetail,
  useInviteMember,
  useCancelInvite,
  useUpdateMemberRole,
  useRemoveMember
} from '@/queries/team.query';
import { useToast } from '@/components/ui/use-toast';
import {
  Plus,
  MoreVertical,
  Crown,
  Shield,
  User,
  UserMinus,
  Mail,
  X
} from 'lucide-react';
import __helpers from '@/helpers';

interface ProjectMembersManagementProps {
  teamId: number;
  canManage: boolean;
}

type TeamRole = 'OWNER' | 'ADMIN' | 'MEMBER';

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

export default function ProjectMembersManagement({
  teamId,
  canManage
}: ProjectMembersManagementProps) {
  const { toast } = useToast();
  const [isInviteOpen, setIsInviteOpen] = useState(false);
  const [inviteEmail, setInviteEmail] = useState('');

  const { data: membersData } = useGetTeamMembers(teamId);
  const { data: teamDetailData } = useGetTeamDetail(teamId);
  const inviteMemberMutation = useInviteMember();
  const cancelInviteMutation = useCancelInvite();
  const updateRoleMutation = useUpdateMemberRole();
  const removeMemberMutation = useRemoveMember();

  const members = membersData?.data || membersData || [];
  const teamDetail = teamDetailData?.data || teamDetailData;
  const pendingInvites = teamDetail?.pendingInvites || [];

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const getRole = (member: any): TeamRole => {
    if (typeof member.role === 'string') {
      return member.role as TeamRole;
    }
    return ROLE_MAP[member.role] || 'MEMBER';
  };

  const handleInvite = async () => {
    if (!inviteEmail.trim()) {
      toast({
        title: 'Error',
        description: 'Email is required',
        variant: 'destructive'
      });
      return;
    }

    try {
      await inviteMemberMutation.mutateAsync({
        teamId,
        email: inviteEmail.trim()
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Invitation sent successfully'
      });
      setIsInviteOpen(false);
      setInviteEmail('');
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to send invitation',
        variant: 'destructive'
      });
    }
  };

  const handleCancelInvite = async (inviteId: number) => {
    try {
      await cancelInviteMutation.mutateAsync({
        teamId,
        inviteId
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Invitation cancelled'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to cancel invitation',
        variant: 'destructive'
      });
    }
  };

  const handleUpdateRole = async (memberId: number, newRole: TeamRole) => {
    try {
      await updateRoleMutation.mutateAsync({
        teamId,
        memberId,
        role: ROLE_TO_NUMBER[newRole]
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Role updated successfully'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error?.response?.data?.message || 'Failed to update role',
        variant: 'destructive'
      });
    }
  };

  const handleRemoveMember = async (memberId: number) => {
    if (!confirm('Are you sure you want to remove this member?')) {
      return;
    }

    try {
      await removeMemberMutation.mutateAsync({
        teamId,
        memberId
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Member removed successfully'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to remove member',
        variant: 'destructive'
      });
    }
  };

  return (
    <div className="space-y-6">
      {/* Members Section */}
      <div>
        <div className="mb-4 flex items-center justify-between">
          <h3 className="text-lg font-semibold">Team Members</h3>
          {canManage && (
            <Dialog open={isInviteOpen} onOpenChange={setIsInviteOpen}>
              <DialogTrigger asChild>
                <Button size="sm">
                  <Plus className="mr-2 h-4 w-4" />
                  Invite Member
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Invite Team Member</DialogTitle>
                  <DialogDescription>
                    Send an invitation to join this team
                  </DialogDescription>
                </DialogHeader>
                <div className="space-y-4 py-4">
                  <div className="space-y-2">
                    <label htmlFor="email" className="text-sm font-medium">
                      Email
                    </label>
                    <Input
                      id="email"
                      type="email"
                      value={inviteEmail}
                      onChange={(e) => setInviteEmail(e.target.value)}
                      placeholder="user@example.com"
                    />
                  </div>
                </div>
                <DialogFooter>
                  <Button
                    variant="outline"
                    onClick={() => {
                      setIsInviteOpen(false);
                      setInviteEmail('');
                    }}
                  >
                    Cancel
                  </Button>
                  <Button
                    onClick={handleInvite}
                    disabled={inviteMemberMutation.isPending}
                  >
                    Send Invitation
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>
          )}
        </div>

        <div className="space-y-2">
          {members.map((member: any) => {
            const role = getRole(member);
            const RoleIcon = ROLE_CONFIG[role].icon;
            const isOwner = role === 'OWNER';

            return (
              <div
                key={member.id}
                className="flex items-center justify-between rounded-lg border p-3"
              >
                <div className="flex items-center gap-3">
                  <Avatar className="h-10 w-10">
                    <AvatarFallback>
                      {getInitials(member.name || member.email || 'U')}
                    </AvatarFallback>
                  </Avatar>
                  <div>
                    <p className="font-medium">{member.name || member.email}</p>
                    <p className="text-sm text-muted-foreground">
                      {member.email}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <Badge
                    variant="outline"
                    className={ROLE_CONFIG[role].bgColor}
                  >
                    <RoleIcon className="mr-1 h-3 w-3" />
                    {ROLE_CONFIG[role].label}
                  </Badge>
                  {canManage && !isOwner && (
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" size="sm">
                          <MoreVertical className="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <Select
                          value={role}
                          onValueChange={(value) =>
                            handleUpdateRole(member.id, value as TeamRole)
                          }
                        >
                          <SelectTrigger className="w-full border-0">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectItem value="MEMBER">Member</SelectItem>
                            <SelectItem value="ADMIN">Admin</SelectItem>
                            {isOwner && (
                              <SelectItem value="OWNER">Owner</SelectItem>
                            )}
                          </SelectContent>
                        </Select>
                        <DropdownMenuItem
                          onClick={() => handleRemoveMember(member.id)}
                          className="text-destructive"
                        >
                          <UserMinus className="mr-2 h-4 w-4" />
                          Remove
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Pending Invites Section */}
      {canManage && pendingInvites.length > 0 && (
        <div>
          <h3 className="mb-4 text-lg font-semibold">Pending Invitations</h3>
          <div className="space-y-2">
            {pendingInvites.map((invite: any) => (
              <div
                key={invite.id}
                className="flex items-center justify-between rounded-lg border p-3"
              >
                <div className="flex items-center gap-3">
                  <Mail className="h-5 w-5 text-muted-foreground" />
                  <div>
                    <p className="font-medium">{invite.email}</p>
                    <p className="text-sm text-muted-foreground">
                      Invited by {invite.createdByName}
                    </p>
                  </div>
                </div>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => handleCancelInvite(invite.id)}
                  className="text-destructive hover:text-destructive"
                >
                  <X className="mr-2 h-4 w-4" />
                  Cancel
                </Button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
