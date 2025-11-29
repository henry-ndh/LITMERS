import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
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
  useGetProjectLabels,
  useCreateProjectLabel,
  useUpdateProjectLabel,
  useDeleteProjectLabel
} from '@/queries/issue.query';
import { useToast } from '@/components/ui/use-toast';
import { Plus, Edit, Trash2, Tag } from 'lucide-react';
import { Badge } from '@/components/ui/badge';

interface ProjectLabelsManagementProps {
  projectId: number;
}

export default function ProjectLabelsManagement({
  projectId
}: ProjectLabelsManagementProps) {
  const { toast } = useToast();
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingLabel, setEditingLabel] = useState<any>(null);
  const [name, setName] = useState('');
  const [color, setColor] = useState('#3B82F6');

  const { data: labelsData, isLoading } = useGetProjectLabels(projectId);
  const createLabelMutation = useCreateProjectLabel();
  const updateLabelMutation = useUpdateProjectLabel();
  const deleteLabelMutation = useDeleteProjectLabel();

  const labels = labelsData?.data || labelsData || [];

  const handleCreate = async () => {
    if (!name.trim()) {
      toast({
        title: 'Error',
        description: 'Label name is required',
        variant: 'destructive'
      });
      return;
    }

    try {
      await createLabelMutation.mutateAsync({
        projectId,
        name: name.trim(),
        color
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Label created successfully'
      });
      setIsCreateOpen(false);
      setName('');
      setColor('#3B82F6');
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error?.response?.data?.message || 'Failed to create label',
        variant: 'destructive'
      });
    }
  };

  const handleStartEdit = (label: any) => {
    setEditingLabel(label);
    setName(label.name);
    setColor(label.color);
  };

  const handleUpdate = async () => {
    if (!name.trim()) {
      toast({
        title: 'Error',
        description: 'Label name is required',
        variant: 'destructive'
      });
      return;
    }

    try {
      await updateLabelMutation.mutateAsync({
        labelId: editingLabel.id,
        projectId,
        name: name.trim(),
        color
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Label updated successfully'
      });
      setEditingLabel(null);
      setName('');
      setColor('#3B82F6');
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error?.response?.data?.message || 'Failed to update label',
        variant: 'destructive'
      });
    }
  };

  const handleDelete = async (labelId: number) => {
    if (!confirm('Are you sure you want to delete this label?')) {
      return;
    }

    try {
      await deleteLabelMutation.mutateAsync({
        labelId,
        projectId
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Label deleted successfully'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error?.response?.data?.message || 'Failed to delete label',
        variant: 'destructive'
      });
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-semibold">Project Labels</h3>
        <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
          <DialogTrigger asChild>
            <Button size="sm">
              <Plus className="mr-2 h-4 w-4" />
              Create Label
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Create Label</DialogTitle>
              <DialogDescription>
                Create a new label for this project
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="name">Name</Label>
                <Input
                  id="name"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Label name"
                  maxLength={30}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="color">Color</Label>
                <div className="flex items-center gap-2">
                  <Input
                    id="color"
                    type="color"
                    value={color}
                    onChange={(e) => setColor(e.target.value)}
                    className="h-10 w-20"
                  />
                  <Input
                    value={color}
                    onChange={(e) => setColor(e.target.value)}
                    placeholder="#3B82F6"
                    maxLength={7}
                  />
                </div>
              </div>
            </div>
            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => {
                  setIsCreateOpen(false);
                  setName('');
                  setColor('#3B82F6');
                }}
              >
                Cancel
              </Button>
              <Button
                onClick={handleCreate}
                disabled={createLabelMutation.isPending}
              >
                Create
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      {isLoading ? (
        <p className="text-sm text-muted-foreground">Loading labels...</p>
      ) : labels.length === 0 ? (
        <p className="text-sm text-muted-foreground">No labels yet</p>
      ) : (
        <div className="space-y-2">
          {labels.map((label: any) => (
            <div
              key={label.id}
              className="flex items-center justify-between rounded-lg border p-3"
            >
              <div className="flex items-center gap-3">
                <Badge
                  variant="outline"
                  style={{
                    backgroundColor: `${label.color}20`,
                    borderColor: label.color,
                    color: label.color
                  }}
                >
                  <Tag className="mr-1 h-3 w-3" />
                  {label.name}
                </Badge>
              </div>
              <div className="flex items-center gap-2">
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => handleStartEdit(label)}
                >
                  <Edit className="h-4 w-4" />
                </Button>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => handleDelete(label.id)}
                  className="text-destructive hover:text-destructive"
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Edit Dialog */}
      <Dialog
        open={!!editingLabel}
        onOpenChange={(open) => !open && setEditingLabel(null)}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Label</DialogTitle>
            <DialogDescription>Update label information</DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="edit-name">Name</Label>
              <Input
                id="edit-name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Label name"
                maxLength={30}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-color">Color</Label>
              <div className="flex items-center gap-2">
                <Input
                  id="edit-color"
                  type="color"
                  value={color}
                  onChange={(e) => setColor(e.target.value)}
                  className="h-10 w-20"
                />
                <Input
                  value={color}
                  onChange={(e) => setColor(e.target.value)}
                  placeholder="#3B82F6"
                  maxLength={7}
                />
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setEditingLabel(null)}>
              Cancel
            </Button>
            <Button
              onClick={handleUpdate}
              disabled={updateLabelMutation.isPending}
            >
              Update
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
