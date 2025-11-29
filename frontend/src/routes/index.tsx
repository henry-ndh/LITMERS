import GoogleCallback from '@/pages/auth/google-call-back';
import NotFound from '@/pages/not-found';
import { Suspense, lazy } from 'react';
import { Navigate, Outlet, useRoutes } from 'react-router-dom';
// import ProtectedRoute from './ProtectedRoute';

const DashboardLayout = lazy(
  () => import('@/components/layout/dashboard-layout')
);

const HomePage = lazy(() => import('@/pages/HomePage/index'));
const AcceptInvitePage = lazy(
  () => import('@/pages/TeamPage/AcceptInvitePage')
);
const ForgotPasswordPage = lazy(() => import('@/pages/auth/forgot-password'));
const ResetPasswordPage = lazy(() => import('@/pages/auth/reset-password'));
const ProfilePage = lazy(() => import('@/pages/ProfilePage/index'));
const ProjectPage = lazy(() => import('@/pages/ProjectPage/index'));
const ProjectDetailPage = lazy(
  () => import('@/pages/ProjectPage/ProjectDetailPage')
);
const IssueDetailPage = lazy(() => import('@/pages/IssuePage/IssueDetailPage'));
const WorkspacePage = lazy(() => import('@/pages/WorkspacePage/index'));
// ----------------------------------------------------------------------

export default function AppRouter() {
  const dashboardRoutes = [
    {
      path: '/',
      element: (
        <Suspense>
          <WorkspacePage />
        </Suspense>
      ),
      index: true
    },
    {
      element: (
        <DashboardLayout>
          <Suspense>
            <Outlet />
          </Suspense>
        </DashboardLayout>
      ),
      children: [
        {
          path: '/login',
          element: <HomePage />
        },

        {
          path: '/team/accept-invite',
          element: <AcceptInvitePage />
        },
        {
          path: '/project',
          element: <ProjectPage />,
          index: true
        },
        {
          path: '/project/:projectId/issue/:issueId',
          element: <IssueDetailPage />
        },
        {
          path: '/project/:projectId',
          element: <ProjectDetailPage />
        },
        {
          path: '/profile',
          element: <ProfilePage />
        }
      ]
    }
  ];

  const publicRoutes = [
    {
      path: '/auth/forgot-password',
      element: (
        <Suspense>
          <ForgotPasswordPage />
        </Suspense>
      )
    },
    {
      path: '/reset-password',
      element: (
        <Suspense>
          <ResetPasswordPage />
        </Suspense>
      )
    },
    {
      path: '/api/auth/google-callback',
      element: <GoogleCallback />
    },
    {
      path: '/404',
      element: <NotFound />
    },
    {
      path: '*',
      element: <Navigate to="/404" replace />
    }
  ];

  const routes = useRoutes([...dashboardRoutes, ...publicRoutes]);

  return routes;
}
