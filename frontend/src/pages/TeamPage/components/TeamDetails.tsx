import { useState, useEffect } from 'react';
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
import { Alert, AlertDescription } from '@/components/ui/alert';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger
} from '@/components/ui/alert-dialog';
import { Pencil, Trash2, Save, X, AlertTriangle } from 'lucide-react';
import { useUpdateTeam, useDeleteTeam } from '@/queries/team.query';
import { toast } from 'sonner';
import { useNavigate } from 'react-router-dom';

interface Team {
  id: string;
  name: string;
  owner_id: string;
  created_at: string;
  updated_at: string;
}

interface TeamDetailsProps {
  team: Team;
  currentUserId: string;
}

export default function TeamDetails({ team, currentUserId }: TeamDetailsProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [teamName, setTeamName] = useState(team?.name || '');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const updateTeamMutation = useUpdateTeam();
  const deleteTeamMutation = useDeleteTeam();

  if (!team || !team.id) {
    return (
      <div className="text-center text-muted-foreground">
        Không tìm thấy thông tin team
      </div>
    );
  }

  const isOwner = team.owner_id === currentUserId;

  useEffect(() => {
    if (team?.name) {
      setTeamName(team.name);
    }
  }, [team?.name]);

  const handleSave = async () => {
    if (!teamName.trim()) {
      setError('Tên team không được để trống');
      return;
    }

    setError('');

    try {
      await updateTeamMutation.mutateAsync({ id: team.id, name: teamName });
      toast.success('Cập nhật team thành công');
      setIsEditing(false);
    } catch (err: any) {
      setError(
        err?.response?.data?.message || 'Có lỗi xảy ra khi cập nhật team'
      );
      toast.error('Có lỗi xảy ra khi cập nhật team');
    }
  };

  const handleCancel = () => {
    setTeamName(team?.name || '');
    setIsEditing(false);
    setError('');
  };

  const handleDelete = async () => {
    try {
      await deleteTeamMutation.mutateAsync(team.id);
      toast.success('Xóa team thành công');
      navigate('/team');
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Có lỗi xảy ra khi xóa team');
      toast.error('Có lỗi xảy ra khi xóa team');
    }
  };

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Thông tin Team</CardTitle>
          <CardDescription>Quản lý thông tin cơ bản của team</CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          {error && (
            <Alert variant="destructive">
              <AlertTriangle className="h-4 w-4" />
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="teamName">Tên Team</Label>
              {isEditing ? (
                <div className="flex gap-2">
                  <Input
                    id="teamName"
                    value={teamName}
                    onChange={(e) => setTeamName(e.target.value)}
                    placeholder="Nhập tên team"
                    maxLength={50}
                    disabled={updateTeamMutation.isPending}
                    className="max-w-md"
                  />
                  <Button
                    onClick={handleSave}
                    disabled={updateTeamMutation.isPending}
                    size="sm"
                  >
                    <Save className="mr-2 h-4 w-4" />
                    Lưu
                  </Button>
                  <Button
                    onClick={handleCancel}
                    disabled={updateTeamMutation.isPending}
                    variant="outline"
                    size="sm"
                  >
                    <X className="mr-2 h-4 w-4" />
                    Hủy
                  </Button>
                </div>
              ) : (
                <div className="flex items-center gap-2">
                  <p className="text-sm font-medium">
                    {team?.name || 'Unnamed Team'}
                  </p>
                  {isOwner && (
                    <Button
                      onClick={() => setIsEditing(true)}
                      variant="ghost"
                      size="sm"
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              )}
              <p className="text-xs text-muted-foreground">
                {teamName.length}/50 ký tự
              </p>
            </div>

            <div className="grid grid-cols-1 gap-4 pt-4 md:grid-cols-2">
              <div className="space-y-1">
                <Label className="text-xs text-muted-foreground">Team ID</Label>
                <p className="font-mono text-sm">{team?.id || 'N/A'}</p>
              </div>
              <div className="space-y-1">
                <Label className="text-xs text-muted-foreground">
                  Ngày tạo
                </Label>
                <p className="text-sm">
                  {team?.created_at
                    ? new Date(team.created_at).toLocaleDateString('vi-VN', {
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                      })
                    : 'N/A'}
                </p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {isOwner && (
        <Card className="border-destructive">
          <CardHeader>
            <CardTitle className="text-destructive">Vùng nguy hiểm</CardTitle>
            <CardDescription>Các hành động không thể hoàn tác</CardDescription>
          </CardHeader>
          <CardContent>
            <AlertDialog>
              <AlertDialogTrigger asChild>
                <Button variant="destructive" className="gap-2">
                  <Trash2 className="h-4 w-4" />
                  Xóa Team
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>
                    Bạn có chắc chắn muốn xóa team này?
                  </AlertDialogTitle>
                  <AlertDialogDescription>
                    Hành động này không thể hoàn tác. Tất cả dữ liệu liên quan
                    đến team bao gồm các project, issue, và hoạt động sẽ bị xóa
                    vĩnh viễn.
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel disabled={deleteTeamMutation.isPending}>
                    Hủy
                  </AlertDialogCancel>
                  <AlertDialogAction
                    onClick={handleDelete}
                    disabled={deleteTeamMutation.isPending}
                    className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                  >
                    {deleteTeamMutation.isPending ? 'Đang xóa...' : 'Xóa Team'}
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
