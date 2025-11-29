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
import { Label } from '@/components/ui/label';
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
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle
} from '@/components/ui/alert-dialog';
import { Alert, AlertDescription } from '@/components/ui/alert';
import {
  UserPlus,
  Mail,
  Clock,
  CheckCircle2,
  XCircle,
  Copy,
  Trash2,
  AlertTriangle
} from 'lucide-react';
import {
  useInviteMember,
  useCancelInvite,
  useGetTeamDetail
} from '@/queries/team.query';
import { toast } from 'sonner';

interface TeamInvite {
  id: number;
  teamId: number;
  teamName: string;
  email: string;
  token: string;
  expiresAt: string;
  acceptedAt: string | null;
  createdBy: number;
  createdByName: string;
  createdAt: string;
  isExpired: boolean;
  isAccepted: boolean;
}

interface TeamInvitesProps {
  teamId: string;
  currentUserId: string;
}

export default function TeamInvites({
  teamId,
  currentUserId
}: TeamInvitesProps) {
  const [isInviteDialogOpen, setIsInviteDialogOpen] = useState(false);
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [inviteToDelete, setInviteToDelete] = useState<TeamInvite | null>(null);
  const [copiedToken, setCopiedToken] = useState<string | null>(null);

  const { data: teamDetailData, isLoading } = useGetTeamDetail(teamId);
  const inviteMemberMutation = useInviteMember();
  const cancelInviteMutation = useCancelInvite();

  const teamDetail = teamDetailData?.data || teamDetailData;
  const invites: TeamInvite[] = teamDetail?.pendingInvites || [];

  const handleInvite = async () => {
    if (!email.trim()) {
      setError('Please enter an email');
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      setError('Invalid email address');
      return;
    }

    setError('');

    try {
      await inviteMemberMutation.mutateAsync({ teamId, email });
      toast.success('Invitation sent successfully');
      setEmail('');
      setIsInviteDialogOpen(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to send invitation');
      toast.error('Failed to send invitation');
    }
  };

  const handleDeleteInvite = async () => {
    if (!inviteToDelete) return;

    try {
      await cancelInviteMutation.mutateAsync({
        teamId,
        inviteId: inviteToDelete.id
      });
      toast.success('Invitation deleted successfully');
      setInviteToDelete(null);
    } catch (err: any) {
      toast.error(
        err?.response?.data?.message || 'Failed to delete invitation'
      );
    }
  };

  const handleCopyLink = (token: string) => {
    const inviteLink = `${window.location.origin}/team/accept-invite?token=${token}`;
    navigator.clipboard.writeText(inviteLink);
    setCopiedToken(token);
    setTimeout(() => setCopiedToken(null), 2000);
    toast.success('Invite link copied!');
  };

  const getInviteStatus = (invite: TeamInvite) => {
    if (invite.isAccepted || invite.acceptedAt) {
      return {
        label: 'Accepted',
        icon: CheckCircle2,
        color: 'text-green-600',
        bgColor: 'bg-green-50 border-green-200'
      };
    }

    if (invite.isExpired || new Date(invite.expiresAt) < new Date()) {
      return {
        label: 'Expired',
        icon: XCircle,
        color: 'text-red-600',
        bgColor: 'bg-red-50 border-red-200'
      };
    }

    return {
      label: 'Pending',
      icon: Clock,
      color: 'text-amber-600',
      bgColor: 'bg-amber-50 border-amber-200'
    };
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInHours = Math.floor(
      (now.getTime() - date.getTime()) / (1000 * 60 * 60)
    );

    if (diffInHours < 24) {
      if (diffInHours < 1) return 'Just now';
      return `${diffInHours} hour${diffInHours > 1 ? 's' : ''} ago`;
    }

    const diffInDays = Math.floor(diffInHours / 24);
    if (diffInDays < 7) {
      return `${diffInDays} day${diffInDays > 1 ? 's' : ''} ago`;
    }

    return date.toLocaleDateString('en-US', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  };

  const pendingInvites = invites.filter(
    (inv) => !inv.isAccepted && !inv.isExpired
  );
  const expiredOrAccepted = invites.filter(
    (inv) => inv.isAccepted || inv.isExpired
  );

  return (
    <>
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Team Invitations</CardTitle>
              <CardDescription>
                Invite new members to join your team
              </CardDescription>
            </div>
            <Dialog
              open={isInviteDialogOpen}
              onOpenChange={setIsInviteDialogOpen}
            >
              <DialogTrigger asChild>
                <Button className="gap-2">
                  <UserPlus className="h-4 w-4" />
                  Invite Member
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Invite New Member</DialogTitle>
                  <DialogDescription>
                    Send an invitation via email. The invitation will be valid
                    for 7 days.
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
                    <Label htmlFor="email">Email</Label>
                    <Input
                      id="email"
                      type="email"
                      placeholder="user@example.com"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      disabled={inviteMemberMutation.isPending}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter') {
                          handleInvite();
                        }
                      }}
                    />
                  </div>
                </div>

                <DialogFooter>
                  <Button
                    variant="outline"
                    onClick={() => {
                      setIsInviteDialogOpen(false);
                      setEmail('');
                      setError('');
                    }}
                    disabled={inviteMemberMutation.isPending}
                  >
                    Cancel
                  </Button>
                  <Button
                    onClick={handleInvite}
                    disabled={inviteMemberMutation.isPending}
                  >
                    {inviteMemberMutation.isPending
                      ? 'Sending...'
                      : 'Send Invitation'}
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>
          </div>
        </CardHeader>
        <CardContent className="space-y-6">
          {isLoading ? (
            <div className="py-12 text-center text-muted-foreground">
              Loading...
            </div>
          ) : (
            <>
              {/* Pending Invites */}
              {pendingInvites.length > 0 && (
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <h3 className="text-sm font-medium">Pending Invitations</h3>
                    <Badge variant="secondary">{pendingInvites.length}</Badge>
                  </div>
                  <div className="divide-y rounded-lg border">
                    {pendingInvites.map((invite) => {
                      const status = getInviteStatus(invite);
                      const StatusIcon = status.icon;

                      return (
                        <div
                          key={invite.id}
                          className="flex items-center justify-between p-4 transition-colors hover:bg-muted/50"
                        >
                          <div className="flex min-w-0 flex-1 items-center gap-3">
                            <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-full bg-primary/10">
                              <Mail className="h-5 w-5 text-primary" />
                            </div>
                            <div className="min-w-0 flex-1">
                              <p className="truncate font-medium">
                                {invite.email}
                              </p>
                              <p className="text-sm text-muted-foreground">
                                Invited by {invite.createdByName || 'N/A'} •{' '}
                                {formatDate(invite.createdAt)}
                              </p>
                            </div>
                          </div>

                          <div className="flex items-center gap-2">
                            <Badge
                              variant="outline"
                              className={`${status.bgColor} ${status.color}`}
                            >
                              <StatusIcon className="mr-1 h-3 w-3" />
                              {status.label}
                            </Badge>

                            <Button
                              variant="outline"
                              size="sm"
                              onClick={() => handleCopyLink(invite.token)}
                              className="gap-2"
                            >
                              <Copy className="h-4 w-4" />
                              {copiedToken === invite.token
                                ? 'Copied!'
                                : 'Copy link'}
                            </Button>

                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => setInviteToDelete(invite)}
                              className="text-destructive hover:text-destructive"
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}

              {/* Expired or Accepted Invites */}
              {expiredOrAccepted.length > 0 && (
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <h3 className="text-sm font-medium">History</h3>
                    <Badge variant="secondary">
                      {expiredOrAccepted.length}
                    </Badge>
                  </div>
                  <div className="divide-y rounded-lg border bg-muted/30">
                    {expiredOrAccepted.map((invite) => {
                      const status = getInviteStatus(invite);
                      const StatusIcon = status.icon;

                      return (
                        <div
                          key={invite.id}
                          className="flex items-center justify-between p-4 opacity-75"
                        >
                          <div className="flex min-w-0 flex-1 items-center gap-3">
                            <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-full bg-muted">
                              <Mail className="h-5 w-5 text-muted-foreground" />
                            </div>
                            <div className="min-w-0 flex-1">
                              <p className="truncate font-medium text-muted-foreground">
                                {invite.email}
                              </p>
                              <p className="text-sm text-muted-foreground">
                                {formatDate(invite.createdAt)}
                              </p>
                            </div>
                          </div>

                          <div className="flex items-center gap-2">
                            <Badge variant="outline" className={status.bgColor}>
                              <StatusIcon className="mr-1 h-3 w-3" />
                              {status.label}
                            </Badge>

                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => setInviteToDelete(invite)}
                              className="text-muted-foreground hover:text-destructive"
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}

              {invites.length === 0 && (
                <div className="rounded-lg border bg-muted/30 py-12 text-center">
                  <Mail className="mx-auto mb-3 h-12 w-12 text-muted-foreground" />
                  <p className="text-muted-foreground">No invitations yet</p>
                  <p className="mt-1 text-sm text-muted-foreground">
                    Click "Invite Member" to get started
                  </p>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>

      <AlertDialog
        open={!!inviteToDelete}
        onOpenChange={() => setInviteToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Invitation</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete the invitation to{' '}
              <strong>{inviteToDelete?.email}</strong>? The invite link will no
              longer be valid.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteInvite}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Delete Invitation
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
