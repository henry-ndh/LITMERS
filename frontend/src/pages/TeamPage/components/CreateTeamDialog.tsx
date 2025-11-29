import { useState, ReactNode } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { AlertTriangle, Users, Loader2 } from 'lucide-react';
import { useCreateTeam } from '@/queries/team.query';
import { toast } from 'sonner';

interface CreateTeamDialogProps {
  children?: ReactNode;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export default function CreateTeamDialog({
  children,
  open: controlledOpen,
  onOpenChange: controlledOnOpenChange
}: CreateTeamDialogProps) {
  const [internalOpen, setInternalOpen] = useState(false);
  const [teamName, setTeamName] = useState('');
  const [error, setError] = useState('');

  const isControlled = controlledOpen !== undefined;
  const open = isControlled ? controlledOpen : internalOpen;
  const setOpen = isControlled ? controlledOnOpenChange! : setInternalOpen;

  const createTeamMutation = useCreateTeam();

  const handleCreate = async () => {
    if (!teamName.trim()) {
      setError('Please enter a team name');
      return;
    }

    setError('');

    try {
      await createTeamMutation.mutateAsync({ name: teamName });
      toast.success('Team created successfully');
      setTeamName('');
      setOpen(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create team');
      toast.error('Failed to create team');
    }
  };

  const handleClose = () => {
    setOpen(false);
    setTeamName('');
    setError('');
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      {children && <DialogTrigger asChild>{children}</DialogTrigger>}
      <DialogContent className="sm:max-w-[480px]">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50">
              <Users className="h-5 w-5 text-blue-600" />
            </div>
            <div>
              <DialogTitle className="text-xl text-slate-800">
                Create New Team
              </DialogTitle>
              <DialogDescription className="mt-1 text-slate-600">
                Start collaborating with your team members
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-5 py-4">
          {error && (
            <Alert
              variant="destructive"
              className="border-red-200 bg-red-50 animate-in fade-in slide-in-from-top-2"
            >
              <AlertTriangle className="h-4 w-4 text-red-600" />
              <AlertDescription className="text-red-800">
                {error}
              </AlertDescription>
            </Alert>
          )}

          <div className="space-y-3">
            <Label
              htmlFor="teamName"
              className="text-sm font-semibold text-slate-700"
            >
              Team Name <span className="text-red-500">*</span>
            </Label>
            <Input
              id="teamName"
              placeholder="e.g., Marketing Team, Engineering, Design..."
              value={teamName}
              onChange={(e) => {
                setTeamName(e.target.value);
                setError('');
              }}
              disabled={createTeamMutation.isPending}
              maxLength={50}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && !createTeamMutation.isPending) {
                  handleCreate();
                }
              }}
              className="h-11 border-slate-300 focus-visible:ring-blue-500"
              autoFocus
            />
            <div className="flex items-center justify-between">
              <p className="text-xs text-slate-500">
                Choose a clear, descriptive name
              </p>
              <p className="text-xs text-slate-500">{teamName.length}/50</p>
            </div>
          </div>
        </div>

        <DialogFooter className="gap-2 sm:gap-0">
          <Button
            variant="outline"
            onClick={handleClose}
            disabled={createTeamMutation.isPending}
            className="w-full border-slate-300 text-slate-700 hover:bg-slate-50 sm:w-auto"
          >
            Cancel
          </Button>
          <Button
            onClick={handleCreate}
            disabled={createTeamMutation.isPending || !teamName.trim()}
            className="w-full bg-blue-600 text-white hover:bg-blue-700 sm:w-auto"
          >
            {createTeamMutation.isPending ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Creating...
              </>
            ) : (
              <>
                <Users className="mr-2 h-4 w-4" />
                Create Team
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
